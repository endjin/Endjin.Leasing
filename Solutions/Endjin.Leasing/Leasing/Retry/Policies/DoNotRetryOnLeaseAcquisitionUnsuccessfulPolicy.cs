namespace Endjin.Leasing.Retry.Policies
{
    #region Using Directives

    using System;

    using Endjin.Core.Retry.Policies;
    using Endjin.Leasing.Exceptions;

    #endregion

    /// <summary>
    /// Retry policy that will retry unless a HTTP 409 Conflict status code is detected.
    /// </summary>
    /// <remarks>This indicates the lease is currently locked.</remarks>
    public class DoNotRetryOnLeaseAcquisitionUnsuccessfulPolicy : IRetryPolicy
    {
        /// <summary>
        /// Checks to see if the exception thrown is expected and whether a retry attempt should be made.
        /// </summary>
        /// <param name="exception">Exception generated inside the retry scope.</param>
        /// <returns>Whether a retry attempt should be made.</returns>
        public bool CanRetry(Exception exception)
        {
            var storageException = exception as LeaseAcquisitionUnsuccessfulException;

            return storageException == null;
        }
    }
}