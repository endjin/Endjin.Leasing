namespace Endjin.Leasing
{
    #region Using Directives

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Contracts.Leasing;

    #endregion

    /// <summary>
    /// Describes the behavior for a lease on a distributed system.
    /// </summary>
    public class Lease : IDisposable
    {
        private readonly ILeaseProvider leaseProvider;

        public Lease(ILeaseProvider leaseProvider, ILeasePolicyValidator leasePolicyValidator)
        {
            this.LeasePolicyValidator = leasePolicyValidator;
            this.leaseProvider = leaseProvider;
        }

        /// <summary>
        /// Gets a value indicating when the lease expires
        /// </summary>
        public DateTimeOffset? Expires
        {
            get
            {
                if (!this.LastAcquired.HasValue)
                {
                    return null;
                }

                return !this.LeasePolicy.Duration.HasValue ? DateTimeOffset.MaxValue : this.LastAcquired.Value.Add(this.LeasePolicy.Duration.Value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether a lease is currently acquired.
        /// </summary>
        public bool HasLease
        {
            get { return !this.LeaseHasExpired() && !string.IsNullOrEmpty(this.Id); }
        }

        /// <summary>
        /// Gets the id of the currently acquired lease.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the time the lease was last acquired.
        /// </summary>
        public DateTimeOffset? LastAcquired { get; private set; }

        /// <summary>
        /// Gets the lease policy.
        /// </summary>
        public ILeasePolicy LeasePolicy { get; private set; }

        /// <summary>
        /// Gets a Lease Policy Validator, used to ensure the validity of the lease policy.
        /// </summary>
        public ILeasePolicyValidator LeasePolicyValidator { get; private set; }

        /// <summary>
        /// Attempts to acquire a lease based on the provided lease policy.
        /// </summary>
        /// <param name="leasePolicy">The configuration details for the lease.</param>
        /// <returns>A task for the async operation.</returns>
        public async Task AcquireAsync(ILeasePolicy leasePolicy)
        {
            this.LeasePolicyValidator.Validate(leasePolicy);

            this.LeasePolicy = leasePolicy;

            this.Id = await this.leaseProvider.AcquireAsync(this.LeasePolicy, this.Id);

            if (!string.IsNullOrEmpty(this.Id))
            {
                this.LastAcquired = DateTimeOffset.UtcNow;
            }
        }

        /// <summary>
        /// Dispose of the currently acquired lease
        /// </summary>
        public void Dispose()
        {
#pragma warning disable 4014
            this.DisposeAsync();
#pragma warning restore 4014
        }

        public async Task DisposeAsync()
        {
            GC.SuppressFinalize(this);

            if (this.HasLease)
            {
                Trace.TraceInformation("Lease.Dispose called.");
                await this.ReleaseAsync();
                Trace.TraceInformation("Lease successfully disposed.");
            }
        }

        /// <summary>
        /// Attempts to extend the lease based on the lease policy provided to initially acquire it.
        /// </summary>
        /// <remarks>A valid lease and lease policy must exist for this operation to execute. An InvalidOperationException will be thrown otherwise.</remarks>
        /// <returns>A task for the async operation.</returns>
        public async Task ExtendAsync()
        {
            if (!this.HasLease)
            {
                throw new InvalidOperationException("A lease must first be acquired in order to extend to lease.");
            }

            await this.leaseProvider.ExtendAsync(this.Id);

            this.LastAcquired = DateTimeOffset.UtcNow;
        }

        public async Task ExtendAsync(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await this.ExtendAsync();
            }
        }

        /// <summary>
        /// Attempts to release the currently acquired lease.
        /// </summary>
        /// <returns>A task for the async operation.</returns>
        public async Task ReleaseAsync()
        {
            // todo what happens if we don't have a lease?
            await this.leaseProvider.ReleaseAsync(this.Id);

            this.Id = null;
            this.LastAcquired = null;
        }

        private bool LeaseHasExpired()
        {
            if (!this.LastAcquired.HasValue)
            {
                return false;
            }

            var remaining = DateTimeOffset.UtcNow - this.LastAcquired.Value;

            if (this.LeasePolicy.Duration.HasValue)
            {
                if ((this.LeasePolicy.Duration.Value - remaining).Seconds > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}