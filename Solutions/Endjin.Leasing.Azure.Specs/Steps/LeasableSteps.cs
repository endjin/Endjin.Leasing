namespace Endjin.Leasing.Azure.Specs.Steps
{
    #region Using Directives

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Core.Retry.Policies;
    using Endjin.Core.Retry.Strategies;
    using Endjin.Leasing.Retry.Policies;

    using Should;

    using TechTalk.SpecFlow;

    #endregion 

    [Binding]
    public class LeasableSteps
    {
        private const string LeaseName = "LeaseName";
        private const string TaskDuration = "TaskDuration";
        private const string LeaseDuration = "LeaseDuration";
        private const string Result = "Result";
        private const string RetryPolicy = "RetryPolicy";
        private const string RetryStrategy = "RetryStrategy";

        [Given(@"actor B is currently running the task")]
        public void GivenActorBIsCurrentlyRunningTheTask()
        {
            var leasable = new Leasable();
            var leaseName = ScenarioContext.Current.Get<string>(LeaseName);

            var renewalTask = Task.Factory.StartNew(async () =>
            {
                await leasable.MutexAsync(this.DoSomething, leaseName, "Actor B");
            },
            CancellationToken.None);

            renewalTask.ContinueWith(t => { }, TaskScheduler.Current);

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
        }

        [Given(@"the lease duration is (.*)")]
        public void GivenTheLeaseDurationIsSeconds(TimeSpan duration)
        {
            ScenarioContext.Current.Set(duration, LeaseDuration);
        }

        [Given(@"the lease name is ""(.*)""")]
        public void GivenTheLeaseNameIs(string leaseName)
        {
            ScenarioContext.Current.Set(leaseName, LeaseName);
        }

        [Given(@"the long running task takes (.*) to complete")]
        public void GivenTheLongRunningTaskTakesSecondsToComplete(TimeSpan duration)
        {
            ScenarioContext.Current.Set(duration, TaskDuration);
        }

        [Given(@"we use a do not retry policy")]
        public void GivenWeUseADoNotRetryPolicy()
        {
            var policy = new DoNotRetryPolicy();
            ScenarioContext.Current.Set(policy, RetryPolicy);
        }

        [Given(@"we use no lease policy")]
        public void GivenWeUseNoLeasePolicy()
        {
        }

        [Given(@"we use a do not retry on lease acquisition unsuccessful policy")]
        public void GivenWeUseADoNotRetryOnLeaseAcquisitionUnsuccessfulPolicy()
        {
            var policy = new DoNotRetryOnLeaseAcquisitionUnsuccessfulPolicy();
            ScenarioContext.Current.Set(policy, RetryPolicy);
        }

        [Given(@"we use a linear retry strategy with periodicity of (.*) and (.*) max retries")]
        public void GivenWeUseALinearRetryStrategy(TimeSpan periodicity, int maxRetries)
        {
            var strategy = new Linear(periodicity, maxRetries);
            ScenarioContext.Current.Set(strategy, RetryStrategy);
        }

        [Given(@"we use no retry strategy")]
        public void GivenWeUseNoRetryStrategy()
        {
        }

        [When(@"Actor A executes the task")]
        public void WhenActorAExecutesTheTask()
        {
            var leasable = new Leasable();
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

        [When(@"Actor A executes the task with options")]
        public void WhenActorAExecutesTheTaskWithOptions()
        {
            var leasable = new Leasable();

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

        [When(@"I execute the task")]
        public void WhenIExecuteTheTask()
        {
            var leasable = new Leasable();
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

        [When(@"I execute the task with options")]
        public void WhenIExecuteTheTaskWithOptions()
        {
            var leasable = new Leasable();

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

        [Then(@"(.*) action\(s\) should have completed successfully")]
        public void ThenActionSShouldHaveCompletedSuccessfully(int numberOfActions)
        {
            int actionsCompleted;
            
            if (!ScenarioContext.Current.TryGetValue("ActionsCompleted", out actionsCompleted))
            {
                actionsCompleted = 0;
            }

            numberOfActions.ShouldEqual(actionsCompleted);
        }

        [Then(@"after (.*) seconds")]
        public void ThenAfterSeconds(int seconds)
        {
            Task.Delay(TimeSpan.FromSeconds(seconds)).Wait();
        }

        [Then(@"it should return successfully")]
        public void ThenItShouldReturnSuccessfully()
        {
            var result = ScenarioContext.Current.Get<bool>(Result);
            
            result.ShouldBeTrue();
        }

        [Then(@"it should return unsuccessfully")]
        public void ThenItShouldReturnUnsuccessfully()
        {
            var result = ScenarioContext.Current.Get<bool>(Result);
            
            result.ShouldBeFalse();
        }

        private async Task DoSomething(CancellationToken cancellationToken)
        {
            var duration = ScenarioContext.Current.Get<TimeSpan>(TaskDuration);
            
            await Task.Delay(duration, cancellationToken);

            int actionsCompleted;

            if (!ScenarioContext.Current.TryGetValue("ActionsCompleted", out actionsCompleted))
            {
                actionsCompleted = 0;
            }

            actionsCompleted++;

            ScenarioContext.Current.Set(actionsCompleted, "ActionsCompleted");
        }
    }
}
