namespace Endjin.Leasing
{
    #region Using Directives

    using System;

    using Endjin.Contracts.Leasing;

    #endregion Using Directives

    /// <summary>
    /// Defines various options used in the creation and acquisition of a lease.
    /// </summary>
    public class LeasePolicy : ILeasePolicy
    {
        /// <summary>
        /// Gets or sets the name for the actor requesting the lease.
        /// </summary>
        public string ActorName { get; set; }

        /// <summary>
        /// Gets or sets the duration of the lease.
        /// </summary>
        /// <remarks>Different lease implementation will have different validity rules about the lease duration.</remarks>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Gets or sets the name of the lease.
        /// </summary>
        /// <remarks>Different lease implementation will have different validity rules about the lease name.</remarks>
        public string Name { get; set; }
    }
}