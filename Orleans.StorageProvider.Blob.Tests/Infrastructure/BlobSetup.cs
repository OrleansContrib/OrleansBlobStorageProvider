using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Orleans.StorageProvider.Blob.Tests.Infrastructure
{
    public class BlobSetup : IDisposable
    {
        private CloudBlobContainer container;

        public BlobSetup()
        {
            var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var blobClient = account.CreateCloudBlobClient();
            this.container = blobClient.GetContainerReference("grainstate");
            this.container.CreateIfNotExists();
        }

        public void Dispose()
        {
            this.container.DeleteIfExists();
        }
    }
}