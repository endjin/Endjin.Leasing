namespace Endjin.Leasing.Exceptions
{
    #region Using Directives

    using System;

    using Endjin.Contracts.Leasing;

    #endregion Using Directives

    /// <summary>
    /// Exception that represents that a Lease could not be acquired successfully.
    /// </summary>
    public class LeaseAcquisitionUnsuccessfulException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the LeaseAcquisitionUnsuccessfulException class containing the lease policy that was used during the attempt.
        /// </summary>
        /// <param name="leasePolicy">The lease policy used during the attempt to acquire the lease.</param>
        /// <param name="innerException">Exception that cause the lease to be unsuccessfully acquired.</param>
        public LeaseAcquisitionUnsuccessfulException(ILeasePolicy leasePolicy, Exception innerException) : base(string.Empty, innerException)
        {
            this.LeasePolicy = leasePolicy;
        }

        /// <summary>
        /// Gets the lease policy used during the attempt to acquire the lease.
        /// </summary>
        public ILeasePolicy LeasePolicy { get; private set; }
    }
}