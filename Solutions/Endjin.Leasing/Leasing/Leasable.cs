namespace Endjin.Leasing
{
    #region Using Directives

    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Contracts.Leasing;
    using Endjin.Core.Retry;
    using Endjin.Core.Retry.Policies;
    using Endjin.Core.Retry.Strategies;
    using Endjin.Leasing.Exceptions;
    using Endjin.Leasing.Retry.Policies;

    #endregion Using Directives

    /// <summary>
    /// Describes the behavior for the distributed execution of an action in isolation.
    /// </summary>
    public class Leasable : ILeasable
    {
        private readonly ILeasePolicyValidator leasePolicyValidator;
        private readonly ILeaseProviderFactory leaseProviderFactory;

        public Leasable(ILeaseProviderFactory leaseProviderFactory, ILeasePolicyValidator leasePolicyValidator)
        {
            this.leaseProviderFactory = leaseProviderFactory;
            this.leasePolicyValidator = leasePolicyValidator;
        }

        /// <summary>
        /// Gets or sets the <see cref="ILeasePolicy"/> used in the creation and acquisition of a lease.
        /// </summary>
        public ILeasePolicy LeasePolicy { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRetryPolicy"/> used to determine whether to retry acquisition of the lease
        /// </summary>
        /// <remarks>The two default policies are <see cref="RetryUntilLeaseAcquiredPolicy"/> and <see cref="DoNotRetryPolicy"/></remarks>
        public IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRetryStrategy"/> used to acquire the lease.
        /// </summary>
        public IRetryStrategy RetryStrategy { get; set; }

        public async Task<Lease> GetAutoRenewingLeaseAsync(CancellationToken cancellationToken, string leaseName, string actorName = "")
        {
            var leaseProvider = this.leaseProviderFactory.Create();

            this.LeasePolicy = new LeasePolicy { Duration = leaseProvider.DefaultLeaseDuration, Name = leaseName, ActorName = actorName };
            this.RetryStrategy = new Linear(TimeSpan.FromSeconds(Math.Round(leaseProvider.DefaultLeaseDuration.TotalSeconds / 3)), int.MaxValue);
            this.RetryPolicy = new RetryUntilLeaseAcquiredPolicy();

            this.CheckProperties();

            var lease = new Lease(leaseProvider, this.leasePolicyValidator);

            await Retriable.RetryAsync(async () =>
                {
                    await this.AcquireLeaseAndStartRenewingAsync(cancellationToken, leaseProvider, lease);
                },
                cancellationToken,
                this.RetryStrategy,
                this.RetryPolicy);

            return lease;
        }

        public async Task<Lease> GetAutoRenewingLeaseWithOptionsAsync(CancellationToken cancellationToken, string leaseName, string actorName = "")
        {
            var leaseProvider = this.leaseProviderFactory.Create();

            this.CheckProperties();

            var lease = new Lease(leaseProvider, this.leasePolicyValidator);

            await Retriable.RetryAsync(
                async () =>
                {
                    await this.AcquireLeaseAndStartRenewingAsync(cancellationToken, leaseProvider, lease);
                },
                cancellationToken,
                this.RetryStrategy,
                this.RetryPolicy);

            return lease;
        }

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
                var leaseProvider = this.leaseProviderFactory.Create();

                this.LeasePolicy = new LeasePolicy { Duration = leaseProvider.DefaultLeaseDuration, Name = leaseName, ActorName = actorName };
                this.RetryStrategy = new Linear(TimeSpan.FromSeconds(Math.Round(leaseProvider.DefaultLeaseDuration.TotalSeconds / 3)), int.MaxValue);
                this.RetryPolicy = new RetryUntilLeaseAcquiredPolicy();

                var success = await this.TryAcquireLeaseAndExecuteAsync(action, cancellationTokenSource, leaseProvider);

                return success;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }

        public async Task<Tuple<bool, T>> MutexAsync<T>(Func<CancellationToken, Task<T>> action, string leaseName, string actorName = "")
        {
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var leaseProvider = this.leaseProviderFactory.Create();

                this.LeasePolicy = new LeasePolicy { Duration = leaseProvider.DefaultLeaseDuration, Name = leaseName, ActorName = actorName };
                this.RetryStrategy = new Linear(TimeSpan.FromSeconds(Math.Round(leaseProvider.DefaultLeaseDuration.TotalSeconds / 3)), int.MaxValue);
                this.RetryPolicy = new RetryUntilLeaseAcquiredPolicy();

                var success = await this.TryAcquireLeaseAndExecuteAsync(action, cancellationTokenSource, leaseProvider);

                return success;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }

        public async Task<bool> MutexAsync<T>(Func<CancellationToken, T, Task> action, T arg, string leaseName, string actorName = "")
        {
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var leaseProvider = this.leaseProviderFactory.Create();

                this.LeasePolicy = new LeasePolicy { Duration = leaseProvider.DefaultLeaseDuration, Name = leaseName, ActorName = actorName };
                this.RetryStrategy = new Linear(TimeSpan.FromSeconds(Math.Round(leaseProvider.DefaultLeaseDuration.TotalSeconds / 3)), int.MaxValue);
                this.RetryPolicy = new RetryUntilLeaseAcquiredPolicy();

                var success = await this.TryAcquireLeaseAndExecuteAsync(action, cancellationTokenSource, leaseProvider, arg);

                return success;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Provides a single attempt to create a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <remarks>Creates a default CancellationToken and Retry Strategy, and uses a <see cref="DoNotRetryOnLeaseAcquisitionUnsuccessfulPolicy"/>. If lease acquisition is unsuccessful, no more attempts are tried.</remarks>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseName">The name of the lease</param>
        /// <param name="actorName"></param>
        /// <returns>A task representing the async operation.</returns>
        public async Task<bool> MutexTryOnceAsync(Func<CancellationToken, Task> action, string leaseName, string actorName = "")
        {
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var leaseProvider = this.leaseProviderFactory.Create();

                this.LeasePolicy = new LeasePolicy { Duration = leaseProvider.DefaultLeaseDuration, Name = leaseName, ActorName = actorName };
                this.RetryStrategy = new Linear(TimeSpan.FromSeconds(Math.Round(leaseProvider.DefaultLeaseDuration.TotalSeconds / 3)), int.MaxValue);
                this.RetryPolicy = new DoNotRetryOnLeaseAcquisitionUnsuccessfulPolicy();

                var success = await this.TryAcquireLeaseAndExecuteAsync(action, cancellationTokenSource, leaseProvider);

                return success;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }

        public async Task<bool> MutexTryOnceAsync<T>(Func<CancellationToken, T, Task> action, T arg, string leaseName, string actorName = "")
        {
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var leaseProvider = this.leaseProviderFactory.Create();

                this.LeasePolicy = new LeasePolicy { Duration = leaseProvider.DefaultLeaseDuration, Name = leaseName, ActorName = actorName };
                this.RetryStrategy = new Linear(TimeSpan.FromSeconds(Math.Round(leaseProvider.DefaultLeaseDuration.TotalSeconds / 3)), int.MaxValue);
                this.RetryPolicy = new DoNotRetryOnLeaseAcquisitionUnsuccessfulPolicy();

                var success = await this.TryAcquireLeaseAndExecuteAsync(action, cancellationTokenSource, leaseProvider, arg);

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
                var leaseProvider = this.leaseProviderFactory.Create();

                var success = await this.TryAcquireLeaseAndExecuteAsync(action, cancellationTokenSource, leaseProvider);

                return success;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }

        public async Task<bool> MutexWithOptionsAsync<T>(Func<CancellationToken, T, Task> action, T arg)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var leaseProvider = this.leaseProviderFactory.Create();

                var success = await this.TryAcquireLeaseAndExecuteAsync(action, cancellationTokenSource, leaseProvider, arg);

                return success;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }
        }

        private async Task AcquireLeaseAndExecuteInnerAsync(Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource, ILeaseProvider leaseProvider)
        {
            using (var lease = new Lease(leaseProvider, this.leasePolicyValidator))
            {
                try
                {
                    await this.AcquireLeaseAndStartRenewingAsync(cancellationTokenSource.Token, leaseProvider, lease);

                    await action(cancellationTokenSource.Token);

                    Trace.TraceInformation("[{0}] Action completed using lease policy name : {1}", this.LeasePolicy.ActorName, this.LeasePolicy.Name);

                    cancellationTokenSource.Cancel();
                }
                catch (LeaseAcquisitionUnsuccessfulException)
                {
                    Trace.TraceInformation("[{0}] Lease could not be obtained with policy name : {1}.", this.LeasePolicy.ActorName, this.LeasePolicy.Name);
                    throw;
                }
            }
        }

        private async Task<T> AcquireLeaseAndExecuteInnerAsync<T>(Func<CancellationToken, Task<T>> action, CancellationTokenSource cancellationTokenSource, ILeaseProvider leaseProvider)
        {
            using (var lease = new Lease(leaseProvider, this.leasePolicyValidator))
            {
                try
                {
                    await this.AcquireLeaseAndStartRenewingAsync(cancellationTokenSource.Token, leaseProvider, lease);

                    var result = await action(cancellationTokenSource.Token);

                    Trace.TraceInformation("[{0}] Action completed using lease policy name : {1}", this.LeasePolicy.ActorName, this.LeasePolicy.Name);

                    cancellationTokenSource.Cancel();

                    return result;
                }
                catch (LeaseAcquisitionUnsuccessfulException)
                {
                    Trace.TraceInformation("[{0}] Lease could not be obtained with policy name : {1}.", this.LeasePolicy.ActorName, this.LeasePolicy.Name);
                    throw;
                }
            }
        }

        private async Task AcquireLeaseAndExecuteInnerAsync<T>(Func<CancellationToken, T, Task> action, CancellationTokenSource cancellationTokenSource, ILeaseProvider leaseProvider, T arg)
        {
            using (var lease = new Lease(leaseProvider, this.leasePolicyValidator))
            {
                try
                {
                    await this.AcquireLeaseAndStartRenewingAsync(cancellationTokenSource.Token, leaseProvider, lease);

                    await action(cancellationTokenSource.Token, arg);

                    Trace.TraceInformation("[{0}] Action completed using lease policy name : {1}", this.LeasePolicy.ActorName, this.LeasePolicy.Name);

                    cancellationTokenSource.Cancel();
                }
                catch (LeaseAcquisitionUnsuccessfulException)
                {
                    Trace.TraceInformation("[{0}] Lease could not be obtained with policy name : {1}.", this.LeasePolicy.ActorName, this.LeasePolicy.Name);
                    throw;
                }
            }
        }

        private async Task AcquireLeaseAndStartRenewingAsync(CancellationToken cancellationToken, ILeaseProvider leaseProvider, Lease lease)
        {
            Trace.TraceInformation("[{0}] Attempting to acquire a Lease with a lease policy name : {1} - duration: {2}",
                this.LeasePolicy.ActorName, this.LeasePolicy.Name, this.LeasePolicy.Duration);

            await lease.AcquireAsync(this.LeasePolicy);

            Trace.TraceInformation(
                "[{0}] Lease was successfully acquired for lease policy name : {1} - duration: {2}",
                this.LeasePolicy.ActorName,
                this.LeasePolicy.Name,
                this.LeasePolicy.Duration);

            var leaseDuration = this.LeasePolicy.Duration.HasValue
                ? this.LeasePolicy.Duration.Value.Add(TimeSpan.FromSeconds(-5))
                : leaseProvider.DefaultLeaseDuration.Add(TimeSpan.FromSeconds(-1));

            var renewEvery = TimeSpan.FromSeconds(Math.Round(leaseDuration.TotalSeconds / 3));

            this.AcquireRenewingLease(cancellationToken, lease, renewEvery);
        }

        private void AcquireRenewingLease(CancellationToken cancellationToken, Lease lease, TimeSpan renewEvery)
        {
            var renewalTask = Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.WaitHandle.WaitOne(renewEvery);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    Trace.TraceInformation("[{0}] Attempting to extend the Lease using the existing lease policy name : {1} - duration: {2}", this.LeasePolicy.ActorName, this.LeasePolicy.Name, this.LeasePolicy.Duration);

                    await lease.ExtendAsync(cancellationToken);

                    Trace.TraceInformation("[{0}] Lease was successfully extended for lease policy name : {1} - duration: {2}", this.LeasePolicy.ActorName, this.LeasePolicy.Name, this.LeasePolicy.Duration);
                }
            },
            cancellationToken);

            renewalTask.ContinueWith(t => { }, TaskScheduler.Current);
        }

        private void CheckProperties()
        {
            if (this.RetryStrategy == null)
            {
                throw new NullReferenceException("You must provide a valid RetryStrategy property.");
            }

            if (this.RetryPolicy == null)
            {
                throw new NullReferenceException("You must provide a valid RetryPolicy property.");
            }

            if (this.LeasePolicy == null)
            {
                throw new NullReferenceException("You must provide a valid LeasePolicy property.");
            }
        }

        private async Task<bool> TryAcquireLeaseAndExecuteAsync(Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource, ILeaseProvider leaseProvider)
        {
            try
            {
                this.CheckProperties();

                await Retriable.RetryAsync(
                        async () =>
                        {
                            await this.AcquireLeaseAndExecuteInnerAsync(action, cancellationTokenSource, leaseProvider);
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

        private async Task<Tuple<bool, T>> TryAcquireLeaseAndExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationTokenSource cancellationTokenSource, ILeaseProvider leaseProvider)
        {
            try
            {
                this.CheckProperties();

                var result = await Retriable.RetryAsync(
                        () => this.AcquireLeaseAndExecuteInnerAsync(action, cancellationTokenSource, leaseProvider),
                        cancellationTokenSource.Token,
                        this.RetryStrategy,
                        this.RetryPolicy);

                return new Tuple<bool, T>(true, result);
            }
            catch (LeaseAcquisitionUnsuccessfulException)
            {
                return new Tuple<bool, T>(false, default(T));
            }
        }

        private async Task<bool> TryAcquireLeaseAndExecuteAsync<T>(Func<CancellationToken, T, Task> action, CancellationTokenSource cancellationTokenSource, ILeaseProvider leaseProvider, T arg)
        {
            try
            {
                this.CheckProperties();

                await Retriable.RetryAsync(
                        async () =>
                        {
                            await this.AcquireLeaseAndExecuteInnerAsync(action, cancellationTokenSource, leaseProvider, arg);
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
    }
}