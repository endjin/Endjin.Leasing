namespace Endjin.Contracts.Leasing.Azure.Configuration
{
    public interface IConnectionStringProvider
    {
        string ConnectionStringKey { get; }
    }
}