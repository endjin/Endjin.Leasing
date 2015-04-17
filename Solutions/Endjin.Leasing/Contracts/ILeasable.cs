namespace Endjin.Contracts
{
    #region Using Directives

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Core.Retry.Strategies;

    #endregion 

    /// <summary>
    /// Describes the behavior for the distributed execution of an action in isolation.
    /// </summary>
    public interface ILeasable
    {
        /// <summary>
        /// Gets or sets the LeasePolicy to be used during execution of the action.
        /// </summary>
        ILeasePolicy LeasePolicy { get; set; }

        /// <summary>
        /// Gets or sets the Retry Strategy to be used during execution of the action.
        /// </summary>
        IRetryStrategy RetryStrategy { get; set; }

        /// <summary>
        /// Provides a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <param name="action">An async action to execute</param>
        /// <returns>A task representing the async operation.</returns>
        Task<bool> MutexWithOptionsAsync(Func<CancellationToken, Task> action);

        /// <summary>
        /// Provides a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <remarks>Creates a default CancellationToken, LeasePolicy and Retry Strategy.</remarks>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseName">The name of the lease</param>
        /// <param name="actorName">The name of the actor requesting the lease</param>
        /// <returns>A task representing the async operation.</returns>
        Task<bool> MutexAsync(Func<CancellationToken, Task> action, string leaseName, string actorName = "");
    }
}