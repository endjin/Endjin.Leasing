namespace Endjin.Leasing.Azure.Specs.Steps
{
    #region Using Directives

    using System;
    using System.Threading.Tasks;

    using Endjin.Leasing.Azure.Specs.Configuration;

    using Should;

    using TechTalk.SpecFlow;

    #endregion

    [Binding]
    public class LeaseSteps
    {
        [Given(@"I am the only actor trying to perform an operation called ""(.*)""")]
        public void GivenIAmTheOnlyActorTryingToPerformAnOperationCalled(string leaseName)
        {
            var policy = new LeasePolicy { Name = leaseName };

            ScenarioContext.Current.Set(policy);
        }

        [Given(@"I want to acquire a lease for (.*) seconds")]
        public void GivenIWantToAcquireALeaseForSeconds(int leaseDurationInSeconds)
        {
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            policy.Duration = TimeSpan.FromSeconds(leaseDurationInSeconds);
            
            ScenarioContext.Current.Set(policy);
        }

        [When(@"I acquire the lease")]
        public void WhenIAcquireTheLease()
        {
            var policy = ScenarioContext.Current.Get<LeasePolicy>();

            Lease lease;
            
            if (!ScenarioContext.Current.TryGetValue(out lease))
            {
                lease = new Lease(new LeasePolicyValidator(), new LeaseProvider(new ConnectionStringProvider()));
            }

            try
            {
                lease.AcquireAsync(policy).Wait();
            }
            catch (AggregateException ex)
            {
                ScenarioContext.Current.Add("AggregateException", ex);
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Add("Exception", ex);
            }

            ScenarioContext.Current.Set(lease);
        }

        [Then(@"it should retain the lease for (.*)")]
        public void ThenItShouldRetainTheLeaseForSeconds(TimeSpan duration)
        {
            var lease = ScenarioContext.Current.Get<Lease>();

            lease.HasLease.ShouldBeTrue();
            
            Task.Delay(duration.Add(TimeSpan.FromSeconds(-5))).Wait();
            
            lease.HasLease.ShouldBeTrue();
        }

        [Then(@"the lease should be expired after (.*)")]
        public void ThenTheLeaseShouldBeExpiredAfterSeconds(TimeSpan duration)
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            
            Task.Delay(duration).Wait();
            
            lease.HasLease.ShouldBeFalse();
        }

        [Given(@"I have already acquired the lease")]
        public void GivenIHaveAlreadyAcquiredTheLease()
        {
            var lease = new Lease(new LeasePolicyValidator(), new LeaseProvider(new ConnectionStringProvider()));
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            lease.AcquireAsync(policy).Wait();
            
            ScenarioContext.Current.Set(lease);
        }

        [When(@"I renew the lease")]
        public void WhenIRenewTheLease()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            try
            {
                lease.ExtendAsync().Wait();
            }
            catch (AggregateException ex)
            {
                ScenarioContext.Current.Add("AggregateException", ex);
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Add("Exception", ex);
            }

            ScenarioContext.Current.Set(lease);
        }

        [Given(@"the lease has expired")]
        public void GivenTheLeaseHasExpired()
        {
            var lease = new Lease(new LeasePolicyValidator(), new LeaseProvider(new ConnectionStringProvider()));
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            
            lease.AcquireAsync(policy).Wait();

            policy.Duration.HasValue.ShouldBeTrue();

            if (policy.Duration.HasValue)
            {
                Task.Delay(policy.Duration.Value.Add(TimeSpan.FromSeconds(2))).Wait();
            }
            
            ScenarioContext.Current.Set(lease);
        }

        [When(@"I release the lease")]
        public void WhenIReleaseTheLease()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            try
            {
                lease.ReleaseAsync().Wait();
            }
            catch (AggregateException ex)
            {
                ScenarioContext.Current.Add("AggregateException", ex);
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Add("Exception", ex);
            }

            ScenarioContext.Current.Set(lease);
        }

        [Then(@"the lease should no longer be acquired")]
        public void ThenTheLeaseShouldNoLongerBeAcquired()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            
            lease.Id.ShouldBeNull();
        }

        [Then(@"the lease should be expired")]
        public void ThenTheLeaseShouldBeExpired()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            
            lease.HasLease.ShouldBeFalse();
        }

        [Then(@"the lease expiration date should be null")]
        public void ThenTheLeaseExpirationDateShouldBeNull()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            
            lease.Expires.ShouldBeNull();
        }

        [Then(@"the lease last acquired date should be null")]
        public void ThenTheLeaseLastAcquiredDateShouldBeNull()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            
            lease.LastAcquired.ShouldBeNull();
        }

        [When(@"I dispose the lease")]
        public void WhenIDisposeTheLease()
        {
            var lease = ScenarioContext.Current.Get<Lease>();

            try
            {
                lease.Dispose();
            }
            catch (AggregateException ex)
            {
                ScenarioContext.Current.Add("AggregateException", ex);
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Add("Exception", ex);
            }

            ScenarioContext.Current.Set(lease);
        }

        [Then(@"it should not throw an exception")]
        public void ThenItShouldNotThrowAnException()
        {
            Exception exception;
            AggregateException aggregateException;
            
            ScenarioContext.Current.TryGetValue("Exception", out exception).ShouldBeFalse();
            ScenarioContext.Current.TryGetValue("AggregateException", out aggregateException).ShouldBeFalse();
        }

        [Given(@"I am actor B trying to perform an operation called ""(.*)""")]
        public void GivenIAmActorBTryingToPerformAnOperationCalled(string leaseName)
        {
            var policy = new LeasePolicy { Name = leaseName };

            ScenarioContext.Current.Set(policy);
        }

        [Given(@"Actor A has already acquired a lease for an operation called ""(.*)""")]
        public void GivenActorAHasAlreadyAcquiredALeaseForAnOperationCalled(string leaseName)
        {
            var policy = new LeasePolicy { Name = leaseName, Duration = TimeSpan.FromSeconds(15)};
            var lease = new Lease(new LeasePolicyValidator(), new LeaseProvider(new ConnectionStringProvider()));
            
            lease.AcquireAsync(policy).Wait();
            
            ScenarioContext.Current.Set(lease, "ActorALease");
        }

        [Then(@"I should not have acquired the lease")]
        public void ThenIShouldNotHaveAcquiredTheLease()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            
            lease.HasLease.ShouldBeFalse();
        }

        [Then(@"the lease ID should not be set")]
        public void ThenTheLeaseIdShouldNotBeSet()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            
            lease.Id.ShouldBeNull();
        }

        [Then(@"the lease acquired date should not be set")]
        public void ThenTheLeaseAcquiredDateShouldNotBeSet()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            
            lease.LastAcquired.ShouldBeNull();
        }

        [Then(@"the lease expires date should not be set")]
        public void ThenTheLeaseExpiresDateShouldNotBeSet()
        {
            var lease = ScenarioContext.Current.Get<Lease>();

            lease.Expires.ShouldBeNull();
        }
    }
}