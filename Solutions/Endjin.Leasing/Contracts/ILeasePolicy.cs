namespace Endjin.Contracts
{
    using System;

    /// <summary>
    /// Describes the behavior for configuring a lease.
    /// </summary>
    public interface ILeasePolicy
    {
        /// <summary>
        /// Gets or sets the name of the lease.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the duration before the lease expires.
        /// </summary>
        TimeSpan? Duration { get; set; }

        /// <summary>
        /// Gets or sets the name for the actor requesting the lease.
        /// </summary>
        string ActorName { get; set; }
    }
}