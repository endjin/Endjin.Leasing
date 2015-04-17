namespace Endjin.Contracts
{
    #region Using Directives

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    #endregion 

    /// <summary>
    /// Describes the behavior for a lease on a distributed system.
    /// </summary>
    public interface ILease : IDisposable
    {
        /// <summary>
        /// Gets a value indicating when the lease expires
        /// </summary>
        DateTime? Expires { get; }

        /// <summary>
        /// Gets a value indicating whether a lease is currently acquired.
        /// </summary>
        bool HasLease { get; }

        /// <summary>
        /// Gets the id of the currently acquired lease.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the time the lease was last acquired.
        /// </summary>
        DateTime? LastAcquired { get; }

        /// <summary>
        /// Gets the lease policy.
        /// </summary>
        ILeasePolicy LeasePolicy { get; }

        /// <summary>
        /// Gets a Lease Policy Validator, used to ensure the validity of the lease policy.
        /// </summary>
        ILeasePolicyValidator LeasePolicyValidator { get; }

        /// <summary>
        /// Attempts to acquire a lease based on the provided lease policy.
        /// </summary>
        /// <param name="leasePolicy">The configuration details for the lease.</param>
        /// <returns>A task for the async operation.</returns>
        Task AcquireAsync(ILeasePolicy leasePolicy);

        /// <summary>
        /// Attempts to extend the lease based on the lease policy provided to initially acquire it.
        /// </summary>
        /// <remarks>A valid lease and lease policy must exist for this operation to execute. An InvalidOperationException will be thrown otherwise.</remarks>
        /// <returns>A task for the async operation.</returns>
        Task ExtendAsync();

        Task ExtendAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Attempts to release the currently acquired lease.
        /// </summary>
        /// <returns>A task for the async operation.</returns>
        Task ReleaseAsync();
    }
}