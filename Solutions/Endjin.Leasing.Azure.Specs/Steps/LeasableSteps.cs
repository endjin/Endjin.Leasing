namespace Endjin.Leasing.Azure.Specs.Steps
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Core.Retry.Policies;
    using Endjin.Core.Retry.Strategies;
    using Endjin.Leasing;
    using Endjin.Leasing.Azure.Specs.Helpers;
    using Endjin.Leasing.Retry.Policies;

    using NUnit.Framework;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    #endregion

    [Binding]
    public class LeasableSteps
    {
        private const string LeaseDuration = "LeaseDuration";

        private const string LeaseName = "LeaseName";

        private const string LeaseNames = "LeaseNames";

        private const string Result = "Result";

        private const string RetryPolicy = "RetryPolicy";

        private const string RetryStrategy = "RetryStrategy";

        private const string TaskDuration = "TaskDuration";

        private const string TaskResult = "TaskResult";

        private const string Tasks = "Tasks";

        [Given(@"actor A executes the task")]
        public void GivenActorAExecutesTheTask()
        {
            var leasableFactory = new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            var leasable = leasableFactory.Create();
            var leaseName = ScenarioContext.Current.Get<string>(LeaseName);

            var task = leasable.MutexAsync(this.DoSomething, leaseName, "Actor A");
            SetContinuations(task);
            AddToTasks(task);
        }

        [Given(@"actor A executes the task with a try once mutex")]
        public void GivenActorAExecutesTheTaskWithATryOnceMutex()
        {
            var leasableFactory = new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            var leasable = leasableFactory.Create();
            var leaseName = ScenarioContext.Current.Get<string>(LeaseName);

            var task = leasable.MutexTryOnceAsync(this.DoSomething, leaseName, "Actor A");
            SetContinuations(task);
            AddToTasks(task);
        }

        [Given(@"actor A executes the task with options")]
        public void GivenActorAExecutesTheTaskWithOptions()
        {
            var leasableFactory = new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            var leasable = leasableFactory.Create();

            string leaseName;
            TimeSpan leaseDuration;
            IRetryPolicy retryPolicy;
            IRetryStrategy retryStrategy;

            ScenarioContext.Current.TryGetValue(LeaseName, out leaseName);
            ScenarioContext.Current.TryGetValue(LeaseDuration, out leaseDuration);
            ScenarioContext.Current.TryGetValue(RetryPolicy, out retryPolicy);
            ScenarioContext.Current.TryGetValue(RetryStrategy, out retryStrategy);

            leasable.LeasePolicy = new LeasePolicy
            {
                ActorName = "Actor A",
                Duration = leaseDuration,
                Name = leaseName
            };

            leasable.RetryPolicy = retryPolicy;
            leasable.RetryStrategy = retryStrategy;

            var task = leasable.MutexWithOptionsAsync(this.DoSomething);
            SetContinuations(task);
            AddToTasks(task);
        }

        [Given(@"actor B is currently running the task")]
        public void GivenActorBIsCurrentlyRunningTheTask()
        {
            var leasableFactory = new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            var leasable = leasableFactory.Create();
            var leaseName = ScenarioContext.Current.Get<string>(LeaseName);

            var renewalTask = Task.Factory.StartNew(async () =>
            {
                var task = leasable.MutexAsync(this.DoSomething, leaseName, "Actor B");

                AddToTasks(task);

                await task;
            }, CancellationToken.None);

            renewalTask.ContinueWith(t => { }, TaskScheduler.Current);

            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [Given(@"the lease duration is (.*) seconds")]
        public void GivenTheLeaseDurationIsSeconds(int leaseDurationInSeconds)
        {
            ScenarioContext.Current.Set(TimeSpan.FromSeconds(leaseDurationInSeconds), LeaseDuration);
        }

        [Given(@"the lease name is ""(.*)""")]
        public void GivenTheLeaseNameIs(string leaseName)
        {
            ScenarioContext.Current.Set(string.Format("{0}_{1}", leaseName, Guid.NewGuid()), LeaseName);
        }

        [Given(@"the lease names are")]
        public void GivenTheLeaseNamesAre(Table table)
        {
            var leaseNames = table.CreateSet<LeaseName>();
            ScenarioContext.Current.Set(leaseNames, LeaseNames);
        }

        [Given(@"the long running task takes (.*) seconds to complete")]
        public void GivenTheLongRunningTaskTakesSecondsToComplete(int durationInSeconds)
        {
            var duration = TimeSpan.FromSeconds(durationInSeconds);

            ScenarioContext.Current.Set(duration, TaskDuration);
        }

        [Given(@"we use a do not retry on lease acquisition unsuccessful policy")]
        public void GivenWeUseADoNotRetryOnLeaseAcquisitionUnsuccessfulPolicy()
        {
            var policy = new DoNotRetryOnLeaseAcquisitionUnsuccessfulPolicy();
            ScenarioContext.Current.Set(policy, RetryPolicy);
        }

        [Given(@"we use a do not retry policy")]
        public void GivenWeUseADoNotRetryPolicy()
        {
            var policy = new DoNotRetryPolicy();
            ScenarioContext.Current.Set(policy, RetryPolicy);
        }

        [Given(@"we use a linear retry strategy with periodicity of (.*) seconds and (.*) max retries")]
        public void GivenWeUseALinearRetryStrategy(int periodicityInSeconds, int maxRetries)
        {
            var strategy = new Linear(TimeSpan.FromSeconds(periodicityInSeconds), maxRetries);
            ScenarioContext.Current.Set(strategy, RetryStrategy);
        }

        [Given(@"we use no lease policy")]
        public void GivenWeUseNoLeasePolicy()
        {
        }

        [Given(@"we use no retry strategy")]
        public void GivenWeUseNoRetryStrategy()
        {
        }

        [Then(@"(.*) action\(s\) should have completed successfully")]
        public void ThenActionSShouldHaveCompletedSuccessfully(int numberOfActions)
        {
            int actionsCompleted;

            if (!ScenarioContext.Current.TryGetValue("ActionsCompleted", out actionsCompleted))
            {
                actionsCompleted = 0;
            }

            Assert.AreEqual(numberOfActions, actionsCompleted);
        }

        [Then(@"after (.*) seconds")]
        public void ThenAfterSeconds(int seconds)
        {
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }

        [Then(@"all leases should be disposed")]
        public void ThenAllLeasesShouldBeDisposed()
        {
            Console.WriteLine("Check here...");
        }

        [Then(@"it should return successfully")]
        public void ThenItShouldReturnSuccessfully()
        {
            var result = ScenarioContext.Current.Get<bool>(Result);
            Assert.True(result);
        }

        [Then(@"it should return unsuccessfully")]
        public void ThenItShouldReturnUnsuccessfully()
        {
            var result = ScenarioContext.Current.Get<bool>(Result);
            Assert.False(result);
        }

        [Then(@"the task result should be correct")]
        public void ThenTheTaskResultShouldBeCorrect()
        {
            var result = ScenarioContext.Current.Get<bool>(TaskResult);
            Assert.True(result);
        }

        [When(@"I execute the task")]
        public void WhenIExecuteTheTask()
        {
            var leasableFactory = new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            var leasable = leasableFactory.Create();
            var leaseName = ScenarioContext.Current.Get<string>(LeaseName);

            bool result = false;

            try
            {
                result = leasable.MutexAsync(this.DoSomething, leaseName, "Actor A").Result;
            }
            catch (AggregateException ex)
            {
                ScenarioContext.Current.Add("AggregateException", ex);
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Add("Exception", ex);
            }

            ScenarioContext.Current.Set(result, Result);
        }

        [When(@"I execute the task using all the leases")]
        public void WhenIExecuteTheTaskUsingAllTheLeases()
        {
            var leaseNames = ScenarioContext.Current.Get<IEnumerable<LeaseName>>(LeaseNames).Select(ln => ln.Name);
            var multiLeasableFactory = new MultiLeasableFactory(new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator()));

            var multiLeasable = multiLeasableFactory.Create();

            multiLeasable.MutexAsync(this.DoSomething, leaseNames, "Actor").Wait();
        }

        [When(@"I execute the task with a result")]
        public void WhenIExecuteTheTaskWithAResult()
        {
            var leasableFactory = new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            var leasable = leasableFactory.Create();
            var leaseName = ScenarioContext.Current.Get<string>(LeaseName);

            var result = new Tuple<bool, bool>(false, false);

            try
            {
                result = leasable.MutexAsync(this.DoSomethingWithResult, leaseName, "Actor A").Result;
            }
            catch (AggregateException ex)
            {
                ScenarioContext.Current.Add("AggregateException", ex);
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Add("Exception", ex);
            }

            ScenarioContext.Current.Set(result.Item1, Result);
            ScenarioContext.Current.Set(result.Item2, TaskResult);
        }

        [When(@"I execute the task with options")]
        public void WhenIExecuteTheTaskWithOptions()
        {
            var leasableFactory = new LeasableFactory(new AzureLeaseProviderFactory(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            var leasable = leasableFactory.Create();

            string leaseName;
            TimeSpan leaseDuration;
            IRetryPolicy retryPolicy;
            IRetryStrategy retryStrategy;

            ScenarioContext.Current.TryGetValue(LeaseName, out leaseName);
            ScenarioContext.Current.TryGetValue(LeaseDuration, out leaseDuration);
            ScenarioContext.Current.TryGetValue(RetryPolicy, out retryPolicy);
            ScenarioContext.Current.TryGetValue(RetryStrategy, out retryStrategy);

            if (leaseName != null || leaseDuration != default(TimeSpan))
            {
                leasable.LeasePolicy = new LeasePolicy
                {
                    ActorName = "Actor A",
                    Duration = leaseDuration,
                    Name = leaseName
                };
            }

            leasable.RetryPolicy = retryPolicy;
            leasable.RetryStrategy = retryStrategy;

            bool result = false;

            try
            {
                result = leasable.MutexWithOptionsAsync(this.DoSomething).Result;
            }
            catch (AggregateException ex)
            {
                ScenarioContext.Current.Add("AggregateException", ex);
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Add("Exception", ex);
            }

            ScenarioContext.Current.Set(result, Result);
        }

        [When(@"the tasks have completed")]
        public void WhenTheTasksHaveCompleted()
        {
            var tasks = ScenarioContext.Current.Get<List<Task>>(Tasks);

            Task.WaitAll(tasks.ToArray());
        }

        private static void AddToTasks(Task<bool> task)
        {
            List<Task> tasks;
            if (ScenarioContext.Current.TryGetValue(Tasks, out tasks))
            {
                tasks.Add(task);
            }
            else
            {
                tasks = new List<Task>
                {
                    task
                };
            }

            ScenarioContext.Current.Set(tasks, Tasks);
        }

        private static void SetContinuations(Task<bool> task)
        {
            task.ContinueWith(t =>
            {
                var exception = t.Exception;
                ScenarioContext.Current.Add(exception.GetType() == typeof(AggregateException) ? "AggregateException" : "Exception", exception);
            }, TaskContinuationOptions.OnlyOnFaulted);

            task.ContinueWith(t => ScenarioContext.Current.Set(t.Result, Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private async Task DoSomething(CancellationToken cancellationToken)
        {
            var duration = ScenarioContext.Current.Get<TimeSpan>(TaskDuration);

            Trace.WriteLine(string.Format("Starting to do something for {0}", duration));
            await Task.Delay(duration, cancellationToken);
            Trace.WriteLine(string.Format("Finished doing something for {0}", duration));

            int actionsCompleted;

            if (!ScenarioContext.Current.TryGetValue("ActionsCompleted", out actionsCompleted))
            {
                actionsCompleted = 0;
            }

            actionsCompleted++;

            ScenarioContext.Current.Set(actionsCompleted, "ActionsCompleted");
        }

        private async Task<bool> DoSomethingWithResult(CancellationToken cancellationToken)
        {
            var duration = ScenarioContext.Current.Get<TimeSpan>(TaskDuration);

            Trace.WriteLine(string.Format("Starting to do something for {0}", duration));
            await Task.Delay(duration, cancellationToken);
            Trace.WriteLine(string.Format("Finished doing something for {0}", duration));

            int actionsCompleted;

            if (!ScenarioContext.Current.TryGetValue("ActionsCompleted", out actionsCompleted))
            {
                actionsCompleted = 0;
            }

            actionsCompleted++;

            ScenarioContext.Current.Set(actionsCompleted, "ActionsCompleted");

            return true;
        }
    }
}