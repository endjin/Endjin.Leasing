namespace Endjin.Leasing.Azure.Specs.Steps
{
    #region Using Directives

    using System;

    using Endjin.Leasing;

    using TechTalk.SpecFlow;

    #endregion



    [Binding]
    public class LeasePolicyValidatorSteps
    {
        [Given(@"has a valid duration")]
        public void GivenHasAValidDuration()
        {
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            policy.Duration = TimeSpan.FromSeconds(30);
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

        [Given(@"the duration on the lease policy is (.*) seconds")]
        public void GivenTheDurationOnTheLeasePolicyIsSeconds(int leaseDurationInSeconds)
        {
            var policy = new LeasePolicy { Duration = TimeSpan.FromSeconds(leaseDurationInSeconds) };
            ScenarioContext.Current.Set(policy);
        }

        [Given(@"the name on the lease policy is ""(.*)""")]
        public void GivenTheNameOnTheLeasePolicyIs(string name)
        {
            var policy = new LeasePolicy { Name = name };

            ScenarioContext.Current.Set(policy);
        }

        [Given(@"the name on the lease policy is null")]
        public void GivenTheNameOnTheLeasePolicyIsNull()
        {
            var policy = new LeasePolicy();

            ScenarioContext.Current.Set(policy);
        }

        [When(@"I validate the policy")]
        public void WhenIValidateThePolicy()
        {
            var policy = ScenarioContext.Current.Get<LeasePolicy>();
            var validator = new AzureLeasePolicyValidator();

            try
            {
                validator.Validate(policy);
            }
            catch (Exception ex)
            {
                ScenarioContext.Current.Add("Exception", ex);
            }
        }
    }
}