namespace Endjin.Leasing.Azure.Specs.Steps
{
    #region Using Directives

    using System;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    #endregion 

    [Binding]
    public class LeasePolicyValidatorSteps
    {
        [Given(@"the duration on the lease policy is (.*)")]
        public void GivenTheDurationOnTheLeasePolicyIsSeconds(TimeSpan duration)
        {
            var policy = new LeasePolicy { Duration = duration };
            
            ScenarioContext.Current.Set(policy);
        }

        [Given(@"has a valid name")]
        public void GivenHasAValidName()
        {
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            
            policy.Name = "validname";

            ScenarioContext.Current.Set(policy);
        }

        [Given(@"the duration on the lease policy is null")]
        public void GivenTheDurationOnTheLeasePolicyIsNull()
        {
            var policy = new LeasePolicy { Duration = null };

            ScenarioContext.Current.Set(policy);
        }

        [When(@"I validate the policy")]
        public void WhenIValidateThePolicy()
        {
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            var validator = new LeasePolicyValidator();

            try
            {
                validator.Validate(policy);
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Add("Exception", ex);
            }
        }

        [Then(@"it should throw a (.*)")]
        public void ThenItShouldThrowAn(string exceptionName)
        {
            var exception = ScenarioContext.Current.Get<Exception>("Exception");

            Assert.AreEqual(exceptionName, exception.GetType().Name);
        }

        [Then(@"it should throw an AggregateException containing (.*)")]
        public void ThenItShouldThrowAnContainingA(string innerExceptionName)
        {
            var exception = ScenarioContext.Current.Get<AggregateException>("AggregateException");

            Assert.AreEqual(innerExceptionName, exception.InnerException.GetType().Name);
        }

        [Then(@"it should not throw any exceptions")]
        public void ThenItShouldNotThrowAnyExceptions()
        {
            Exception exception;
            var hasException = ScenarioContext.Current.TryGetValue("Exception", out exception);
            Assert.False(hasException);
        }

        [Given(@"the name on the lease policy is ""(.*)""")]
        public void GivenTheNameOnTheLeasePolicyIs(string name)
        {
            var policy = new LeasePolicy { Name = name };

            ScenarioContext.Current.Set(policy);
        }

        [Given(@"has a valid duration")]
        public void GivenHasAValidDuration()
        {
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            policy.Duration = TimeSpan.FromSeconds(30);

            ScenarioContext.Current.Set(policy);
        }
    }
}