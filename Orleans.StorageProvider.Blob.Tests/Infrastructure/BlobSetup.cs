using System;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public async Task<T> LoadAsync<T>(string grainId)
        {
            var block = this.container.GetBlockBlobReference(grainId);
            bool exists = await block.ExistsAsync();
            if (!exists)
            {
                return default(T);
            }

            var text = await block.DownloadTextAsync();
            return JsonConvert.DeserializeObject<T>(text);
        }

        public void Dispose()
        {
            this.container.DeleteIfExists();
        }
    }
}