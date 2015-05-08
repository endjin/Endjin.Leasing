namespace Endjin.Leasing
{
    #region Using Directives

    using Endjin.Contracts.Leasing;

    #endregion Using Directives

    public class AzureLeaseProviderFactory : ILeaseProviderFactory
    {
        private readonly IConnectionStringProvider connectionStringProvider;

        public AzureLeaseProviderFactory(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        public ILeaseProvider Create()
        {
            return new AzureLeaseProvider(this.connectionStringProvider);
        }
    }
}