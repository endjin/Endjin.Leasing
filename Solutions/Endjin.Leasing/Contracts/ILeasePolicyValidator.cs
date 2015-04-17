namespace Endjin.Contracts
{
    /// <summary>
    /// Describes the behavior for validating Lease Policies.
    /// </summary>
    public interface ILeasePolicyValidator
    {
        /// <summary>
        /// Validates whether the proposed ILeasePolicy is valid for the given lease implementation.
        /// </summary>
        /// <remarks>
        /// If lease policy is invalid thrown a InvalidArgumentException
        /// </remarks>
        /// <param name="leasePolicy">ILeasePolicy to validate</param>
        void Validate(ILeasePolicy leasePolicy);
    }
}