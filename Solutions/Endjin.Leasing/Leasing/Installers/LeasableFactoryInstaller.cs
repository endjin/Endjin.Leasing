namespace Endjin.Leasing.Installers
{
    #region Using Directives

    using Endjin.Contracts.Leasing;
    using Endjin.Core.Container;

    #endregion Using Directives

    public class LeasableFactoryInstaller : IInstaller
    {
        public void Install(IContainer container)
        {
            container.Register(Component.For<ILeasableFactory>().ImplementedBy<LeasableFactory>());
            container.Register(Component.For<IMultiLeasableFactory>().ImplementedBy<MultiLeasableFactory>());
        }
    }
}