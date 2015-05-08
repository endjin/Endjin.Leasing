namespace Endjin.Leasing.Azure.Specs.Helpers
{
    #region Using Directives

    using Endjin.Contracts.Leasing;

    #endregion

    public class ConnectionStringProvider : IConnectionStringProvider
    {
        public string ConnectionStringKey
        {
            get
            {
                return "Endjin.Leasing.ConnectionString";
            }
        }
    }
}