namespace Endjin.Leasing.Azure
{
    #region Using Directives

    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Endjin.Contracts;
    using Endjin.Contracts.Leasing.Azure.Configuration;
    using Endjin.Core.Retry;
    using Endjin.Core.Retry.Strategies;
    using Endjin.Leasing.Azure.Retry.Policies;
    using Endjin.Leasing.Exceptions;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    #endregion 

    /// <summary>
    /// The platform specific implementation used for lease operations
    /// </summary>
    public class LeaseProvider : ILeaseProvider
    {
        private readonly IConnectionStringProvider connectionStringProvider;

        private CloudBlockBlob blob;
        private bool isInitialised;

        public LeaseProvider(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// Gets the default lease duration for the specific platform implementation of the lease.
        /// </summary>
        public TimeSpan DefaultLeaseDuration
        {
            get { return TimeSpan.FromSeconds(59); }
        }

        /// <summary>
        /// Gets the lease policy.
        /// </summary>
        public ILeasePolicy LeasePolicy { get; private set; }

        /// <summary>
        /// Attempts to acquire a lease based on the provided lease policy.
        /// </summary>
        /// <param name="leasePolicy">The configuration details for the lease.</param>
        /// <param name="leaseId">The id of the currently acquired lease.</param>
        /// <returns>A task for the async operation.</returns>
        public async Task<string> AcquireAsync(ILeasePolicy leasePolicy, string leaseId)
        {
            this.LeasePolicy = leasePolicy;

            await this.InitialiseAsync();

            if (!this.blob.Exists())
            {
                await Retriable.RetryAsync(async () =>
                {
                    using (var ms = new MemoryStream())
                    {
                        await this.blob.UploadFromStreamAsync(ms);
                    }
                });
            }

            try
            {
                return await Retriable.RetryAsync(
                        () => this.blob.AcquireLeaseAsync(leasePolicy.Duration, leaseId),
                        new CancellationToken(),
                        new Count(10),
                        new DoNotRetryOnConflictPolicy());
            }
            catch (StorageException exception)
            {
                if (exception.RequestInformation.HttpStatusCode == 409)
                {
                    throw new LeaseAcquisitionUnsuccessfulException(this.LeasePolicy, exception);
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to extend the lease based on the lease policy provided to initially acquire it.
        /// </summary>
        /// <param name="leaseId">The id of the lease to attempt to extend.</param>
        /// <remarks>A valid lease and lease policy must exist for this operation to execute. An InvalidOperationException will be thrown otherwise.</remarks>
        /// <returns>A task for the async operation.</returns>
        public async Task ExtendAsync(string leaseId)
        {
            await this.InitialiseAsync();
            await Retriable.RetryAsync(() => this.blob.RenewLeaseAsync(new AccessCondition { LeaseId = leaseId }));
        }

        /// <summary>
        /// Attempts to release the currently acquired lease.
        /// </summary>
        /// <param name="leaseId">The id of the lease to attempt to release.</param>
        /// <returns>A task for the async operation.</returns>
        public async Task ReleaseAsync(string leaseId)
        {
            await this.InitialiseAsync();
            await Retriable.RetryAsync(() => this.blob.ReleaseLeaseAsync(new AccessCondition { LeaseId = leaseId }));
        }

        private async Task InitialiseAsync()
        {
            if (!this.isInitialised)
            {
                var storageAccount = CloudStorageAccount.Parse(Configuration.GetSettingFor(this.connectionStringProvider.ConnectionStringKey));
                var client = storageAccount.CreateCloudBlobClient();
                var container = client.GetContainerReference("endjin-leasing-leases");

                await Retriable.RetryAsync(container.CreateIfNotExistsAsync);

                this.blob = container.GetBlockBlobReference(this.LeasePolicy.Name.ToLowerInvariant());

                this.isInitialised = true;
            }
        }
    }
}