namespace Endjin.Leasing
{
    #region Using Directives

    using System;

    using Endjin.Contracts.Leasing;

    #endregion Using Directives

    /// <summary>
    /// Describes the behavior for validating Lease Policies.
    /// </summary>
    public class AzureLeasePolicyValidator : ILeasePolicyValidator
    {
        /// <summary>
        /// Validates whether the proposed ILeasePolicy is valid for the given lease implementation.
        /// </summary>
        /// <remarks>
        /// If lease policy is invalid thrown a InvalidArgumentException
        /// </remarks>
        /// <param name="leasePolicy">ILeasePolicy to validate</param>
        public void Validate(ILeasePolicy leasePolicy)
        {
            if (leasePolicy.Duration.HasValue && (leasePolicy.Duration < TimeSpan.FromSeconds(15) || leasePolicy.Duration > TimeSpan.FromSeconds(59)))
            {
                throw new ArgumentOutOfRangeException("leasePolicy", "Duration proprerty is out of range. Windows Azure finite blob lease must be betweem 15 and 59 seconds");
            }

            if (leasePolicy.Name == null)
            {
                throw new ArgumentNullException("leasePolicy", "Name property must not be null");
            }

            if (leasePolicy.Name.Length < 1 || leasePolicy.Name.Length > 1024)
            {
                throw new ArgumentOutOfRangeException("leasePolicy", "Name property must be between 1 and 1,024 characters.");
            }

            if (leasePolicy.Name.EndsWith(".") || leasePolicy.Name.EndsWith("/"))
            {
                throw new ArgumentException("Name property must not end with a dot ('.') or a forward slash ('/')", "leasePolicy");
            }
        }
    }
}