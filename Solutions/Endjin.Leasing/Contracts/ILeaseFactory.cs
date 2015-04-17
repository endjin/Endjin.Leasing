namespace Endjin.Contracts
{
    /// <summary>
    /// Responsible to creating a valid lease.
    /// </summary>
    public interface ILeaseFactory
    {
        /// <summary>
        /// Gets or sets a Lease Policy Validator, used to ensure the validity of the lease policy.
        /// </summary>
        ILeasePolicyValidator LeasePolicyValidator { get; set; }

        /// <summary>
        /// Gets or sets the Lease Provider, used to provide the platform implementation of the lease.
        /// </summary>
        ILeaseProvider LeaseProvider { get; set; }

        /// <summary>
        /// Attempts create an ILease using the lease policy provided.
        /// </summary>
        /// <returns>A task containing the newly created lease.</returns>
        ILease Create();
    }
}