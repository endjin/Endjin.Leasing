namespace Endjin.Leasing
{
    #region Using Directives

    using Endjin.Contracts.Leasing;

    #endregion

    public class LeasableFactory : ILeasableFactory
    {
        private readonly ILeasePolicyValidator leasePolicyValidator;
        private readonly ILeaseProviderFactory leaseProviderFactory;

        public LeasableFactory(ILeaseProviderFactory leaseProviderFactory, ILeasePolicyValidator leasePolicyValidator)
        {
            this.leaseProviderFactory = leaseProviderFactory;
            this.leasePolicyValidator = leasePolicyValidator;
        }

        public ILeasable Create()
        {
            return new Leasable(this.leaseProviderFactory, this.leasePolicyValidator);
        }
    }
}