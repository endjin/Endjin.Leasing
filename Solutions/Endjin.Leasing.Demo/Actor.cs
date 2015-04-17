namespace Endjin.Leasing.Demo
{
    #region Using Directives

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Core.Composition;
    using Endjin.Core.Retry.Strategies;
    using Endjin.Leasing.Retry.Policies;

    #endregion 

    public class Actor
    {
        public Actor(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        /// <summary>
        /// This will retry until it sucessfully executes.
        /// </summary>
        /// <returns></returns>
        public Task RunSimpleMutexAsync()
        {
            var leasable = new Leasable();

            return leasable.MutexAsync(this.DoSomethingAsync, "LeasingTestHarness", this.Name);
        }

        /// <summary>
        /// This will try and fail if it cannot get the lease first time.
        /// </summary>
        /// <returns></returns>
        public Task RunMutexWithOptionsAsync()
        {
            var leasable = new Leasable
            {
                LeasePolicy = new LeasePolicy { Duration = TimeSpan.FromSeconds(15), Name = "LeasingTestHarness", ActorName = this.Name },
                RetryStrategy = new DoNotRetry(),
                RetryPolicy = new DoNotRetryPolicy()
            };

            return leasable.MutexWithOptionsAsync(this.DoSomethingAsync);
        }

        private async Task DoSomethingAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[{0}] : Started something!", this.Name);

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            
            Console.WriteLine("[{0}] : Finished something!", this.Name);
        }
    }
}