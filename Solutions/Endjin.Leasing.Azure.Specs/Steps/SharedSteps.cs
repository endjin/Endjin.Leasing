namespace Endjin.Leasing.Azure.Specs.Steps
{
    #region Using Directives

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
    }
}