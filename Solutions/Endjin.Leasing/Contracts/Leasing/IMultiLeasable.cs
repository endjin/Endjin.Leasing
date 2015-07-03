namespace Endjin.Contracts.Leasing
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Core.Retry.Policies;
    using Endjin.Core.Retry.Strategies;

    #endregion

    /// <summary>
    /// Describes the behavior for the distributed execution of an action in isolation using multiple leases.
    /// </summary>
    public interface IMultiLeasable
    {
        /// <summary>
        /// Gets or sets the LeasePolicy to be used during execution of the action.
        /// </summary>
        IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Gets or sets the RetryPolicy to be used during execution of the action.
        /// </summary>
        IRetryStrategy RetryStrategy { get; set; }

        /// <summary>
        /// Provides a distributed lock using multiple leases whilst executing the given async action to execute to ensure isolation. 
        /// Locks on all leases are obtained before the action is executed.
        /// </summary>
        /// <remarks>Creates a default CancellationToken, LeasePolicy and Retry Strategy.</remarks>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseNames">The names of the leases</param>
        /// <param name="actorName">The name of the actor requesting the lease</param>
        /// <returns>A task representing the async operation.</returns>
        Task MutexAsync(Func<CancellationToken, Task> action, IEnumerable<string> leaseNames, string actorName = "");

        /// <summary>
        /// Provides a distributed lock using multiple leases whilst executing the given async action to execute to ensure isolation. 
        /// Locks on all leases are obtained before the action is executed.
        /// </summary>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseNames">The names of the leases</param>
        /// <param name="leaseDuration">The duration before the lease expires after acquiring it successfully</param>
        /// <param name="actorName">The name of the actor requesting the lease</param>
        /// <returns>A task representing the async operation.</returns>
        Task MutexWithOptionsAsync(Func<CancellationToken, Task> action, IEnumerable<string> leaseNames, TimeSpan leaseDuration, string actorName = "");
    }
}