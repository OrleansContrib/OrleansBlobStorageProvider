using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Orleans;
using Orleans.Storage;
using System;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace OrleansBlobStorageProvider
{

    public class BlobStorageProvider : IStorageProvider
    {
        CloudBlobContainer container;

        public OrleansLogger Log { get; set; }

        public string Name { get; set; }

        public async Task Init(string name, Orleans.Providers.IProviderRuntime providerRuntime, Orleans.Providers.IProviderConfiguration config)
        {
            try
            {
                var account = CloudStorageAccount.Parse(config.Properties["DataConnectionString"]);
                var blobClient = account.CreateCloudBlobClient();
                var containerName = config.Properties.ContainsKey("ContainerName") ? config.Properties["ContainerName"] : "grainstate";
                container = blobClient.GetContainerReference(containerName);
                await container.CreateIfNotExistsAsync();
            }
            catch (Exception ex)
            {
                Log.Error(0, ex.ToString());
            }

        }

        public Task Close()
        {
            return TaskDone.Done;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainId, IGrainState grainState)
        {
            try
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
            catch (StorageException)
            {

            }
            catch (Exception ex)
            {
                Log.Error(0, ex.ToString());
            }
        }

        private static string GetBlobName(string grainType, GrainReference grainId)
        {
            return string.Format("{0}-{1}.json", grainType, grainId.ToKeyString()); ;
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainId, IGrainState grainState)
        {
            try
            {
                var blobName = GetBlobName(grainType, grainId);
                var storedData = new JavaScriptSerializer().Serialize(grainState.AsDictionary());
                var blob = container.GetBlockBlobReference(blobName);
                await blob.UploadTextAsync(storedData);
            }
            catch (Exception ex)
            {
                Log.Error(0, ex.ToString());
            }
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainId, GrainState grainState)
        {
            try
            {
                var blobName = GetBlobName(grainType, grainId);
                var blob = container.GetBlockBlobReference(blobName);
                await blob.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                Log.Error(0, ex.ToString());
            }
        }
    }

}
