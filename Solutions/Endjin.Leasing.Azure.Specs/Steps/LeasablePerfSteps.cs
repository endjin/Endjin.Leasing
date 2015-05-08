namespace Endjin.Leasing.Azure.Specs.Steps
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Core.Retry.Strategies;
    using Endjin.Leasing;
    using Endjin.Leasing.Azure.Specs.Helpers;
    using Endjin.Leasing.Retry.Policies;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    #endregion

    [Binding]
    public class LeasablePerfSteps
    {
        [Given(@"an action takes (.*) ms")]
        public void GivenAnActionTakesMs(int numberOfMilliseconds)
        {
            ScenarioContext.Current.Set(TimeSpan.FromMilliseconds(numberOfMilliseconds), "duration");
        }

        [Then(@"the result should take less than (.*) second\(s\)")]
        public void ThenTheResultShouldTakeLessThanSecondS(int numberOfSeconds)
        {
            var stopwatch = ScenarioContext.Current.Get<Stopwatch>("stopwatch");

            Console.WriteLine("Total time taken: {0}", stopwatch.Elapsed);

            using (var sw = File.AppendText("leasable-perf-log.txt"))
            {
                sw.WriteLine(ScenarioContext.Current.ScenarioInfo.Title);
                sw.WriteLine("Total time taken: {0}", stopwatch.Elapsed);
                sw.WriteLine("----------------------------------");
            }

            Assert.LessOrEqual(stopwatch.Elapsed, TimeSpan.FromSeconds(numberOfSeconds));
        }

        [When(@"I run (.*) actions consecutively")]
        public void WhenIRunActionsConsecutively(int numberOfActions)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < numberOfActions; i++)
            {
                this.DoSomething(new CancellationToken()).Wait();
            }

            stopwatch.Stop();

            ScenarioContext.Current.Set(stopwatch, "stopwatch");
        }

        [When(@"I run (.*) actions simultaneously")]
        public void WhenIRunActionsSimultaneously(int numberOfActions)
        {
            var stopwatch = new Stopwatch();

            var tasks = new List<Task>();

            for (int i = 0; i < numberOfActions; i++)
            {
                var task = Task.Factory.StartNew(() => this.DoSomething(new CancellationToken()));

                tasks.Add(task);
            }

            stopwatch.Start();
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();

            ScenarioContext.Current.Set(stopwatch, "stopwatch");
        }

        [When(@"I run (.*) actions using leasable")]
        public void WhenIRunActionsUsingLeasable(int numberOfActions)
        {
            var factory = new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator());

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < numberOfActions; i++)
            {
                var leasable = factory.Create();
                leasable.MutexAsync(this.DoSomething, string.Format("testlease{0}", i), "TEST").Wait();
            }

            stopwatch.Stop();

            ScenarioContext.Current.Set(stopwatch, "stopwatch");
        }

        [When(@"I run (.*) mutex actions simultaneously")]
        public void WhenIRunMutexActionsSimultaneously(int numberOfActions)
        {
            var factory = new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator());

            var duration = ScenarioContext.Current.Get<TimeSpan>("duration");

            var stopwatch = new Stopwatch();

            var tasks = new List<Task>();

            for (int i = 0; i < numberOfActions; i++)
            {
                var leasable = factory.Create();

                leasable.RetryPolicy = new RetryUntilLeaseAcquiredPolicy();
                leasable.RetryStrategy = new Linear(TimeSpan.FromSeconds(duration.TotalSeconds / 2), int.MaxValue);
                leasable.LeasePolicy = new LeasePolicy
                {
                    ActorName = "TEST " + i,
                    Duration = TimeSpan.FromSeconds(15),
                    Name = "leasetest"
                };

                var task = leasable.MutexWithOptionsAsync(this.DoSomething);

                tasks.Add(task);
            }

            stopwatch.Start();
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();

            ScenarioContext.Current.Set(stopwatch, "stopwatch");
        }

        private Task DoSomething(CancellationToken token)
        {
            TimeSpan duration;

            ScenarioContext.Current.TryGetValue("duration", out duration);

            if (duration == default(TimeSpan))
            {
                // Do nothing
            }
            else
            {
                Task.Delay(duration, token).Wait(token);
            }

            return Task.FromResult(true);
        }
    }
}