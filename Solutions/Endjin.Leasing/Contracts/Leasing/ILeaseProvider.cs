namespace Endjin.Contracts.Leasing
{
    #region Using Directives

    using System;
    using System.Threading.Tasks;

    #endregion Using Directives

    /// <summary>
    /// The platform specific implementation used for lease operations
    /// </summary>
    public interface ILeaseProvider
    {
        /// <summary>
        /// Gets the default lease duration for the specific platform implementation of the lease.
        /// </summary>
        TimeSpan DefaultLeaseDuration { get; }

        /// <summary>
        /// Gets the lease policy.
        /// </summary>
        ILeasePolicy LeasePolicy { get; }

        /// <summary>
        /// Attempts to acquire a lease based on the provided lease policy.
        /// </summary>
        /// <param name="leasePolicy">The configuration details for the lease.</param>
        /// <param name="leaseId">The id of the currently acquired lease.</param>
        /// <returns>A task for the async operation.</returns>
        Task<string> AcquireAsync(ILeasePolicy leasePolicy, string leaseId);

        /// <summary>
        /// Attempts to extend the lease based on the lease policy provided to initially acquire it.
        /// </summary>
        /// <param name="leaseId">The id of the lease to attempt to extend.</param>
        /// <remarks>A valid lease and lease policy must exist for this operation to execute. An InvalidOperationException will be thrown otherwise.</remarks>
        /// <returns>A task for the async operation.</returns>
        Task ExtendAsync(string leaseId);

        /// <summary>
        /// Attempts to release the currently acquired lease.
        /// </summary>
        /// <param name="leaseId">The id of the lease to attempt to release.</param>
        /// <returns>A task for the async operation.</returns>
        Task ReleaseAsync(string leaseId);
    }
}