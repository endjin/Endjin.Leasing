namespace Endjin.Leasing.Demo.Configuration
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