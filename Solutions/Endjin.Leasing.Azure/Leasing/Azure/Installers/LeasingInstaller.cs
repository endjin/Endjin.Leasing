namespace Endjin.Leasing.Azure.Installers
{
    #region Using Directives

    using Endjin.Contracts.Leasing;
    using Endjin.Core.Container;

    #endregion Using Directives

    public class LeasingInstaller : IInstaller
    {
        public void Install(IContainer container)
        {
            container.Register(Component.For<ILeasePolicyValidator>().ImplementedBy<AzureLeasePolicyValidator>());
            container.Register(Component.For<ILeaseProviderFactory>().ImplementedBy<AzureLeaseProviderFactory>());
        }
    }
}