namespace Endjin.Leasing.Azure.Retry.Policies
{
    #region Using Directives

    using System;

    using Endjin.Core.Retry.Policies;

    using Microsoft.WindowsAzure.Storage;

    #endregion

    /// <summary>
    /// Retry policy that will retry unless a HTTP 409 Conflict status code is detected.
    /// </summary>
    /// <remarks>This indicates the lease is currently locked.</remarks>
    public class DoNotRetryOnConflictPolicy : IRetryPolicy
    {
        /// <summary>
        /// Checks to see if the exception thrown is expected and whether a retry attempt should be made.
        /// </summary>
        /// <param name="exception">Exception generated inside the retry scope.</param>
        /// <returns>Whether a retry attempt should be made.</returns>
        public bool CanRetry(Exception exception)
        {
            var storageException = exception as StorageException;

            return storageException == null || storageException.RequestInformation.HttpStatusCode != 409;
        }
    }
}