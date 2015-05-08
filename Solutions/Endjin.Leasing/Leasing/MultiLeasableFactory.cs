namespace Endjin.Leasing
{
    #region Using Directives

    using Endjin.Contracts.Leasing;

    #endregion Using Directives

    public class MultiLeasableFactory : IMultiLeasableFactory
    {
        private readonly ILeasableFactory leasableFactory;

        public MultiLeasableFactory(ILeasableFactory leasableFactory)
        {
            this.leasableFactory = leasableFactory;
        }

        public IMultiLeasable Create()
        {
            return new MultiLeasable(this.leasableFactory);
        }
    }
}