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

    #endregion Using Directives

    public class MultiLeasable : IMultiLeasable
    {
        private readonly ILeasableFactory leasableFactory;

        private readonly List<Lease> leases = new List<Lease>();

        public MultiLeasable(ILeasableFactory leasableFactory)
        {
            this.leasableFactory = leasableFactory;
        }

        public IRetryPolicy RetryPolicy { get; set; }

        public IRetryStrategy RetryStrategy { get; set; }

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