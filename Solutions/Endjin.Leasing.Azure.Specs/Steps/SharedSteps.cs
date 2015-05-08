namespace Endjin.Storage.Leasing.Azure.Specs.Steps
{
    #region Using Directives

    using System;

    using Endjin.Leasing;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    #endregion


    [Binding]
    public class SharedSteps
    {
        [AfterScenario("ReleaseLeases")]
        public void ReleaseLeases()
        {
            try
            {
                var lease = ScenarioContext.Current.Get<Lease>();
                lease.ReleaseAsync().Wait();
            }
            catch
            {
            }

            try
            {
                var existingLease = ScenarioContext.Current.Get<Lease>("ActorALease");
                existingLease.ReleaseAsync().Wait();
            }
            catch
            {
            }
        }

        [Then(@"it should not throw any exceptions")]
        public void ThenItShouldNotThrowAnyExceptions()
        {
            Exception exception;
            var hasException = ScenarioContext.Current.TryGetValue("Exception", out exception);
            Assert.False(hasException);
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
    }
}