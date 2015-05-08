namespace Endjin.Storage.Leasing.Azure.Specs.Steps
{
    #region Using Directives

    using NUnit.Framework;
    using System;
    using System.Threading;

    using Endjin.Leasing;
    using Endjin.Storage.Leasing.Azure.Specs.Helpers;

    using TechTalk.SpecFlow;

    #endregion

    [Binding]
    public class LeaseSteps
    {
        [Given(@"Actor A has already acquired a lease for an operation called ""(.*)""")]
        public void GivenActorAHasAlreadyAcquiredALeaseForAnOperationCalled(string leaseName)
        {
            var policy = new LeasePolicy { Name = leaseName, Duration = TimeSpan.FromSeconds(15) };
            var lease = new Lease(new AzureLeaseProvider(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            lease.AcquireAsync(policy).Wait();
            ScenarioContext.Current.Set(lease, "ActorALease");
        }

        [Given(@"I am actor B trying to perform an operation called ""(.*)""")]
        public void GivenIAmActorBTryingToPerformAnOperationCalled(string leaseName)
        {
            var policy = new LeasePolicy { Name = leaseName };
            ScenarioContext.Current.Set(policy);
        }

        [Given(@"I am the only actor trying to perform an operation called ""(.*)""")]
        public void GivenIAmTheOnlyActorTryingToPerformAnOperationCalled(string leaseName)
        {
            var policy = new LeasePolicy { Name = leaseName };
            ScenarioContext.Current.Set(policy);
        }

        [Given(@"I have already acquired the lease")]
        public void GivenIHaveAlreadyAcquiredTheLease()
        {
            var lease = new Lease(new AzureLeaseProvider(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            lease.AcquireAsync(policy).Wait();
            ScenarioContext.Current.Set(lease);
        }

        [Given(@"I want to acquire a lease for (.*) seconds")]
        public void GivenIWantToAcquireALeaseForSeconds(int leaseDurationInSeconds)
        {
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            policy.Duration = TimeSpan.FromSeconds(leaseDurationInSeconds);
            ScenarioContext.Current.Set(policy);
        }

        [Given(@"the lease has expired")]
        public void GivenTheLeaseHasExpired()
        {
            var lease = new Lease(new AzureLeaseProvider(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            lease.AcquireAsync(policy).Wait();
            Thread.Sleep(policy.Duration.Value.Add(TimeSpan.FromSeconds(2)));
            ScenarioContext.Current.Set(lease);
        }

        [Then(@"I should not have acquired the lease")]
        public void ThenIShouldNotHaveAcquiredTheLease()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Assert.False(lease.HasLease);
        }

        [Then(@"it should not throw an exception")]
        public void ThenItShouldNotThrowAnException()
        {
            Exception exception;
            AggregateException aggregateException;
            Assert.False(ScenarioContext.Current.TryGetValue("Exception", out exception));
            Assert.False(ScenarioContext.Current.TryGetValue("AggregateException", out aggregateException));
        }

        [Then(@"it should retain the lease for (.*) seconds")]
        public void ThenItShouldRetainTheLeaseForSeconds(int leaseDurationInSeconds)
        {
            var lease = ScenarioContext.Current.Get<Lease>();

            Assert.True(lease.HasLease);
            Thread.Sleep(TimeSpan.FromSeconds(leaseDurationInSeconds - 5));
            Assert.True(lease.HasLease);
        }

        [Then(@"the lease acquired date should not be set")]
        public void ThenTheLeaseAcquiredDateShouldNotBeSet()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Assert.Null(lease.LastAcquired);
        }

        [Then(@"the lease expiration date should be null")]
        public void ThenTheLeaseExpirationDateShouldBeNull()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Assert.Null(lease.Expires);
        }

        [Then(@"the lease expires date should not be set")]
        public void ThenTheLeaseExpiresDateShouldNotBeSet()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Assert.Null(lease.Expires);
        }

        [Then(@"the lease ID should not be set")]
        public void ThenTheLeaseIDShouldNotBeSet()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Assert.Null(lease.Id);
        }

        [Then(@"the lease last acquired date should be null")]
        public void ThenTheLeaseLastAcquiredDateShouldBeNull()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Assert.Null(lease.LastAcquired);
        }

        [Then(@"the lease should be expired")]
        public void ThenTheLeaseShouldBeExpired()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Assert.False(lease.HasLease);
        }

        [Then(@"the lease should be expired after (.*) seconds")]
        public void ThenTheLeaseShouldBeExpiredAfterSeconds(int leaseDurationInSeconds)
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Assert.False(lease.HasLease);
        }

        [Then(@"the lease should expire in the future")]
        public void ThenTheLeaseShouldExpireInTheFuture()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Assert.Greater(lease.Expires, DateTimeOffset.UtcNow);
        }

        [Then(@"the lease should no longer be acquired")]
        public void ThenTheLeaseShouldNoLongerBeAcquired()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            Assert.Null(lease.Id);
        }

        [When(@"I acquire the lease")]
        public void WhenIAcquireTheLease()
        {
            var policy = ScenarioContext.Current.Get<LeasePolicy>();

            Lease lease;
            if (!ScenarioContext.Current.TryGetValue(out lease))
            {
                lease = new Lease(new AzureLeaseProvider(new ConnectionStringProvider()), new AzureLeasePolicyValidator());
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

        [When(@"I dispose the lease")]
        public void WhenIDisposeTheLease()
        {
            var lease = ScenarioContext.Current.Get<Lease>();
            try
            {
                lease.DisposeAsync().Wait();
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
    }
}