namespace Endjin.Leasing
{
    #region Using Directives

    using Endjin.Contracts;

    #endregion

    /// <summary>
    /// Responsible to creating a valid lease.
    /// </summary>
    public class LeaseFactory : ILeaseFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeaseFactory"/> class. 
        /// </summary>
        /// <param name="leasePolicyValidator">The lease policy validator required to validate the lease policy.</param>
        /// <param name="leaseProvider">The platform specific provider for generating a lease.</param>
        public LeaseFactory(ILeasePolicyValidator leasePolicyValidator, ILeaseProvider leaseProvider)
        {
            this.LeasePolicyValidator = leasePolicyValidator;
            this.LeaseProvider = leaseProvider;
        }

        /// <summary>
        /// Gets or sets a Lease Policy Validator, used to ensure the validity of the lease policy.
        /// </summary>
        public ILeasePolicyValidator LeasePolicyValidator { get; set; }

        /// <summary>
        /// Gets or sets the Lease Provider, used to provide the platform implementation of the lease.
        /// </summary>
        public ILeaseProvider LeaseProvider { get; set; }

        /// <summary>
        /// Attempts create an ILease using the lease policy provided.
        /// </summary>
        /// <returns>A task containing the newly created lease.</returns>
        public ILease Create()
        {
            return new Lease(this.LeasePolicyValidator, this.LeaseProvider);
        }
    }
}