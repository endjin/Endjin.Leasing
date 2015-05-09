namespace Endjin.Contracts.Leasing
{
    #region Using Directives

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Core.Retry.Policies;
    using Endjin.Core.Retry.Strategies;
    using Endjin.Leasing;
    using Endjin.Leasing.Retry.Policies;

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
        /// Gets or sets the RetryPolicy to be used during execution of the action.
        /// </summary>
        IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Gets or sets the Retry Strategy to be used during execution of the action.
        /// </summary>
        IRetryStrategy RetryStrategy { get; set; }

        Task<Lease> GetAutoRenewingLeaseAsync(CancellationToken cancellationToken, string leaseName, string actorName = "");

        Task<Lease> GetAutoRenewingLeaseWithOptionsAsync(CancellationToken cancellationToken, string leaseName, string actorName = "");

        /// <summary>
        /// Provides a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <remarks>Creates a default CancellationToken, LeasePolicy and Retry Strategy.</remarks>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseName">The name of the lease</param>
        /// <param name="actorName">The name of the actor requesting the lease</param>
        /// <returns>A task representing the async operation.</returns>
        Task<bool> MutexAsync(Func<CancellationToken, Task> action, string leaseName, string actorName = "");

        /// <summary>
        /// Provides a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">An async action to execute</param>
        /// <param name="arg"></param>
        /// <param name="leaseName">The name of the lease</param>
        /// <param name="actorName">The name of the actor requesting the lease</param>
        /// <returns>A task representing the async operation.</returns>
        Task<bool> MutexAsync<T>(Func<CancellationToken, T, Task> action, T arg, string leaseName, string actorName = "");

        /// <summary>
        /// Provides a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseName">The name of the lease</param>
        /// <param name="actorName">The name of the actor requesting the lease</param>
        /// <returns>A task representing the async operation.</returns>
        Task<Tuple<bool, T>> MutexAsync<T>(Func<CancellationToken, Task<T>> action, string leaseName, string actorName = "");

        /// <summary>
        /// Provides a single attempt to create a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <remarks>Creates a default CancellationToken and Retry Strategy, and uses a <see cref="DoNotRetryOnLeaseAcquisitionUnsuccessfulPolicy"/>. If lease acquisition is unsuccessful, no more attempts are tried.</remarks>
        /// <param name="action">An async action to execute</param>
        /// <param name="leaseName">The name of the lease</param>
        /// <param name="actorName"></param>
        /// <returns>A task representing the async operation.</returns>
        Task<bool> MutexTryOnceAsync(Func<CancellationToken, Task> action, string leaseName, string actorName = "");

        /// <summary>
        /// Provides a distributed lock whilst executing the given async action to execute to ensure isolation.
        /// </summary>
        /// <param name="action">An async action to execute</param>
        /// <returns>A task representing the async operation.</returns>
        Task<bool> MutexWithOptionsAsync(Func<CancellationToken, Task> action);
    }
}