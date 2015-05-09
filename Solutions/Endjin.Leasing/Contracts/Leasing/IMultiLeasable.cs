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

    public interface IMultiLeasable
    {
        IRetryPolicy RetryPolicy { get; set; }

        IRetryStrategy RetryStrategy { get; set; }

        Task MutexAsync(Func<CancellationToken, Task> action, IEnumerable<string> leaseNames, string actorName = "");

        Task MutexWithOptionsAsync(Func<CancellationToken, Task> action, IEnumerable<string> leaseNames, TimeSpan leaseDuration, string actorName = "");
    }
}