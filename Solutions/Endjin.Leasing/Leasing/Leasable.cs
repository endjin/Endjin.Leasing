namespace Endjin.Leasing
{
    #region Using Directives

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Contracts;
    using Endjin.Core.Composition;
    using Endjin.Core.Retry;
    using Endjin.Core.Retry.Policies;
    using Endjin.Core.Retry.Strategies;
    using Endjin.Leasing.Exceptions;
    using Endjin.Leasing.Retry.Policies;

    #endregion

    /// <summary>
    /// Describes the behavior for the distributed execution of an action in isolation.
    /// </summary>
    public class Leasable : ILeasable
    {
        /// <summary>
        /// Gets or sets the <see cref="ILeasePolicy"/> used in the creation and acquisition of a lease.
        /// </summary>
        public ILeasePolicy LeasePolicy { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRetryStrategy"/> used to acquire the lease.
        /// </summary>
        public IRetryStrategy RetryStrategy { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRetryPolicy"/> used to determine whether to retry acquisition of the lease  
        /// </summary>
        /// <remarks>The two default policies are <see cref="RetryUntilLeaseAcquiredPolicy"/> and <see cref="DoNotRetryPolicy"/></remarks>
        public IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Provides simple creation of a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <remarks>Creates a default CancellationToken, LeasePolicy and Retry Strategy.</remarks>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseName">The name of the lease</param>
        /// <param name="actorName"></param>
        /// <returns>A task representing the async operation.</returns>
        public async Task<bool> MutexAsync(Func<CancellationToken, Task> action, string leaseName, string actorName = "")
        {
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var leaseFactory = ApplicationServiceLocator.Container.Resolve<ILeaseFactory>();

                this.LeasePolicy = new LeasePolicy { Duration = leaseFactory.LeaseProvider.DefaultLeaseDuration, Name = leaseName, ActorName = actorName };
                this.RetryStrategy = new Linear(TimeSpan.FromSeconds(Math.Round(leaseFactory.LeaseProvider.DefaultLeaseDuration.TotalSeconds / 3)), int.MaxValue);
                this.RetryPolicy = new RetryUntilLeaseAcquiredPolicy();

                var success = await this.TryAcquireLeaseAndExecuteAsync(action, cancellationTokenSource, leaseFactory);

                if (!success)
                {
                    Trace.TraceInformation(Strings.LeaseCouldNotBeObtainedWithPolicyName, this.LeasePolicy.ActorName, this.LeasePolicy.Name);
                }

                return success;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Provides customizable creation of a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <remarks>Use the <see cref="LeasePolicy"/>, <see cref="RetryStrategy"/> and <see cref="RetryPolicy"/> properties to tailor behavior.</remarks>
        /// <param name="action">An async action to execute</param>
        /// <returns>A task representing the async operation.</returns>
        public async Task<bool> MutexWithOptionsAsync(Func<CancellationToken, Task> action)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var leaseFactory = ApplicationServiceLocator.Container.Resolve<ILeaseFactory>();

                var success = await this.TryAcquireLeaseAndExecuteAsync(action, cancellationTokenSource, leaseFactory);

                return success;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }

        private async Task<bool> TryAcquireLeaseAndExecuteAsync(Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource, ILeaseFactory leaseFactory)
        {
            try
            {
                if (this.RetryStrategy == null)
                {
                    throw new NullReferenceException(Strings.YouMustProvideAValidRetryStrategyProperty);
                }

                if (this.RetryPolicy == null)
                {
                    throw new NullReferenceException(Strings.YouMustProvideAValidRetryPolicyProperty);
                }

                await Retriable.RetryAsync(async () =>
                {
                    await this.AcquireLeaseAndExecuteInnerAsync(action, cancellationTokenSource, leaseFactory);
                },
                cancellationTokenSource.Token,
                this.RetryStrategy,
                this.RetryPolicy);

                return true;
            }
            catch (LeaseAcquisitionUnsuccessfulException)
            {
                return false;
            }
        }

        private async Task AcquireLeaseAndExecuteInnerAsync(Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource, ILeaseFactory leaseFactory)
        {
            if (this.LeasePolicy == null)
            {
                throw new NullReferenceException(Strings.YouMustProvideAValidLeasePolicyProperty);
            }

            using (var lease = leaseFactory.Create())
            {
                Trace.TraceInformation(Strings.AttemptingToAcquireALeaseWithALeasePolicyName, this.LeasePolicy.ActorName, this.LeasePolicy.Name, this.LeasePolicy.Duration);

                try
                {
                    await lease.AcquireAsync(this.LeasePolicy);

                    Trace.TraceInformation(
                        Strings.LeaseWasSuccessfullyAcquiredForLeasePolicyName,
                        this.LeasePolicy.ActorName,
                        this.LeasePolicy.Name,
                        this.LeasePolicy.Duration);

                    var leaseDuration = this.LeasePolicy.Duration.HasValue
                                            ? this.LeasePolicy.Duration.Value.Add(TimeSpan.FromSeconds(-5))
                                            : leaseFactory.LeaseProvider.DefaultLeaseDuration.Add(TimeSpan.FromSeconds(-1));

                    var renewEvery = TimeSpan.FromSeconds(Math.Round(leaseDuration.TotalSeconds / 3));

                    this.AcquireRenewingLease(cancellationTokenSource.Token, lease, renewEvery);

                    await action(cancellationTokenSource.Token);

                    Trace.TraceInformation(Strings.ActionCompletedUsingLeasePolicyName, this.LeasePolicy.ActorName, this.LeasePolicy.Name);

                    cancellationTokenSource.Cancel();
                }
                catch (LeaseAcquisitionUnsuccessfulException)
                {
                    Trace.TraceInformation(Strings.LeaseCouldNotBeObtainedWithPolicyName, this.LeasePolicy.ActorName, this.LeasePolicy.Name);
                    throw;
                }
            }
        }

        private void AcquireRenewingLease(CancellationToken cancellationToken, ILease lease, TimeSpan renewEvery)
        {
            var renewalTask = Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.WaitHandle.WaitOne(renewEvery);

                    Trace.TraceInformation(Strings.AttemptingToExtendTheLeaseUsingTheExistingLeasePolicyName, this.LeasePolicy.ActorName, this.LeasePolicy.Name, this.LeasePolicy.Duration);

                    await lease.ExtendAsync(cancellationToken);

                    Trace.TraceInformation(Strings.LeaseWasSuccessfullyExtendedForLeasePolicyName, this.LeasePolicy.ActorName, this.LeasePolicy.Name, this.LeasePolicy.Duration);
                }
            },
            cancellationToken);

            renewalTask.ContinueWith(t => { }, TaskScheduler.Current);
        }
    }
}