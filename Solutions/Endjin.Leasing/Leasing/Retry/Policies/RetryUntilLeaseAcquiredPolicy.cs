namespace Endjin.Leasing.Retry.Policies
{
    #region Using Directives

    using System;

    using Endjin.Core.Retry.Policies;
    using Endjin.Leasing.Exceptions;

    #endregion Using Directives

    /// <summary>
    /// Retry Policy that will retry until the lease has been successfully acquired.
    /// </summary>
    public class RetryUntilLeaseAcquiredPolicy : IRetryPolicy
    {
        /// <summary>
        /// Checks to see if the exception thrown is expected and whether a retry attempt should be made.
        /// </summary>
        /// <param name="exception">Exception generated inside the retry scope.</param>
        /// <returns>Whether a retry attempt should be made.</returns>
        public bool CanRetry(Exception exception)
        {
            var actualException = exception as LeaseAcquisitionUnsuccessfulException;

            return actualException != null;
        }
    }
}