namespace Endjin.Leasing.Azure.Installers
{
    #region Using Directives

    using Endjin.Core.Installers;

    #endregion

    public class LeasableInstaller : NamespaceInstallerBase<LeasableInstaller>
    {
        public LeasableInstaller() : base("Leasing")
        {
        }
    }
}