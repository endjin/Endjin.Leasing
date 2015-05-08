namespace Endjin.Contracts.Leasing
{
    public interface ILeaseProviderFactory
    {
        ILeaseProvider Create();
    }
}