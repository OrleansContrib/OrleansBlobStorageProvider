using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Orleans;
using Orleans.Storage;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace OrleansStorage
{

    public class BlobStorageProvider : IStorageProvider
    {
        CloudBlobContainer container;

        public OrleansLogger Log { get; set; }

        public string Name { get; set; }

        public Task Init(string name, Orleans.Providers.IProviderRuntime providerRuntime, Orleans.Providers.IProviderConfiguration config)
        {
            var account = CloudStorageAccount.Parse(config.Properties["ConnectionString"]);
            var blobClient = account.CreateCloudBlobClient();
            container = blobClient.GetContainerReference(config.Properties["ContainerName"]);
            return container.CreateIfNotExistsAsync();
        }
        public Task Close()
        {
            return TaskDone.Done;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainId, IGrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainId);
            var blob = container.GetBlockBlobReference(blobName);
            var text = await blob.DownloadTextAsync();
            if (string.IsNullOrWhiteSpace(text))
            {
                var data = new JavaScriptSerializer().Deserialize(text, grainState.GetType());
                var dict = ((IGrainState)data).AsDictionary();
                grainState.SetAll(dict);
            }
        }

        private static string GetBlobName(string grainType, GrainReference grainId)
        {
            return string.Format("{0}-{1}", grainType, grainId.ToKeyString()); ;
        }

        public Task WriteStateAsync(string grainType, GrainReference grainId, IGrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainId);
            var storedData = new JavaScriptSerializer().Serialize(grainState.AsDictionary());
            var blob = container.GetBlockBlobReference(blobName);
            return blob.UploadTextAsync(storedData);
        }

        public Task ClearStateAsync(string grainType, GrainReference grainId, GrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainId);
            var blob = container.GetBlockBlobReference(blobName);
            return blob.DeleteIfExistsAsync();
        }
    }

}
