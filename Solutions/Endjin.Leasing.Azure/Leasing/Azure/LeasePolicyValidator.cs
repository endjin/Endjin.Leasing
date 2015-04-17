namespace Endjin.Leasing.Azure
{
    #region Using Directives

    using System;

    using Endjin.Contracts;

    #endregion 

    /// <summary>
    /// Describes the behavior for validating Lease Policies.
    /// </summary>
    public class LeasePolicyValidator : ILeasePolicyValidator 
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
                throw new ArgumentOutOfRangeException("leasePolicy", Strings.DurationOutOfRangeMessage);
            }

            if (leasePolicy.Name == null)
            {
                throw new ArgumentNullException("leasePolicy", Strings.NamePropertyMustNotBeNull);
            }

            if (leasePolicy.Name.Length < 1 || leasePolicy.Name.Length > 1024)
            {
                throw new ArgumentOutOfRangeException("leasePolicy", Strings.NamePropertyMustBeBetween1And1024Characters);
            }

            if (leasePolicy.Name.EndsWith(".") || leasePolicy.Name.EndsWith("/"))
            {
                throw new ArgumentException(Strings.NamePropertyMustBeValid, "leasePolicy");
            }
        }
    }
}