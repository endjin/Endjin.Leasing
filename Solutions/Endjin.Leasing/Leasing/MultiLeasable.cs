namespace Endjin.Leasing
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Contracts.Leasing;
    using Endjin.Core.Retry.Policies;
    using Endjin.Core.Retry.Strategies;
    using Endjin.Leasing.Retry.Policies;

    #endregion

    public class MultiLeasable : IMultiLeasable
    {
        private readonly ILeasableFactory leasableFactory;
        private readonly List<Lease> leases = new List<Lease>();

        public MultiLeasable(ILeasableFactory leasableFactory)
        {
            this.leasableFactory = leasableFactory;
        }

        /// <summary>
        /// Gets or sets the <see cref="IRetryPolicy"/> used to determine whether to retry acquisition of the lease
        /// </summary>
        /// <remarks>The two default policies are <see cref="RetryUntilLeaseAcquiredPolicy"/> and <see cref="DoNotRetryPolicy"/></remarks>
        public IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRetryStrategy"/> used to acquire the lease.
        /// </summary>
        public IRetryStrategy RetryStrategy { get; set; }

        /// <summary>
        /// Provides a distributed lock using multiple leases whilst executing the given async action to execute to ensure isolation. 
        /// Locks on all leases are obtained before the action is executed.
        /// </summary>
        /// <remarks>Creates a default CancellationToken, LeasePolicy and Retry Strategy.</remarks>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseNames">The names of the leases</param>
        /// <param name="actorName">The name of the actor requesting the lease</param>
        /// <returns>A task representing the async operation.</returns>
        public async Task MutexAsync(Func<CancellationToken, Task> action, IEnumerable<string> leaseNames, string actorName = "")
        {
            var cts = new CancellationTokenSource();

            try
            {
                foreach (var leaseName in leaseNames)
                {
                    var leasable = this.leasableFactory.Create();

                    var lease = await leasable.GetAutoRenewingLeaseAsync(cts.Token, leaseName, actorName);

                    this.leases.Add(lease);
                }

                await action(cts.Token);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            finally
            {
                cts.Cancel();
            }

            await this.ReleaseLeasesAsync();
        }

        /// <summary>
        /// Provides a distributed lock using multiple leases whilst executing the given async action to execute to ensure isolation. 
        /// Locks on all leases are obtained before the action is executed.
        /// </summary>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseNames">The names of the leases</param>
        /// <param name="leaseDuration">The duration before the lease expires after acquiring it successfully</param>
        /// <param name="actorName">The name of the actor requesting the lease</param>
        /// <returns>A task representing the async operation.</returns>
        public async Task MutexWithOptionsAsync(Func<CancellationToken, Task> action, IEnumerable<string> leaseNames, TimeSpan leaseDuration, string actorName = "")
        {
            var cts = new CancellationTokenSource();

            try
            {
                foreach (var leaseName in leaseNames)
                {
                    var leasable = this.leasableFactory.Create();

                    leasable.LeasePolicy = new LeasePolicy
                    {
                        ActorName = actorName,
                        Duration = leaseDuration,
                        Name = leaseName
                    };
                    leasable.RetryPolicy = this.RetryPolicy;
                    leasable.RetryStrategy = this.RetryStrategy;

                    var lease = await leasable.GetAutoRenewingLeaseWithOptionsAsync(cts.Token, leaseName, leaseName);

                    this.leases.Add(lease);
                }

                await action(cts.Token);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            finally
            {
                cts.Cancel();
            }

            await this.ReleaseLeasesAsync();
        }

        private async Task ReleaseLeasesAsync()
        {
            foreach (var lease in this.leases)
            {
                Trace.TraceInformation("[{0}] Attempting to release the Lease for lease policy name : {1}",
                    lease.LeasePolicy.ActorName, lease.LeasePolicy.Name);

                await lease.ReleaseAsync();

                Trace.TraceInformation("[{0}] Successfully released the Lease for lease policy name : {1}",
                    lease.LeasePolicy.ActorName, lease.LeasePolicy.Name);
            }
        }
    }
}