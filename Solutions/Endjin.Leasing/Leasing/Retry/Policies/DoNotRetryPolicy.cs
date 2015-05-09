namespace Endjin.Leasing.Retry.Policies
{
    #region Using Directives

    using System;

    using Endjin.Core.Retry.Policies;

    #endregion

    /// <summary>
    /// Retry Policy that will not retry.
    /// </summary>
    public class DoNotRetryPolicy : IRetryPolicy
    {
        /// <summary>
        /// Do not attempt any retries
        /// </summary>
        /// <param name="exception">Exception generated inside the retry scope.</param>
        /// <returns>Whether a retry attempt should be made.</returns>
        public bool CanRetry(Exception exception)
        {
            return false;
        }
    }
}