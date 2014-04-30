namespace OrleansBlobStorageProvider
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Orleans;
    using Orleans.Providers;
    using Orleans.Storage;
    using Newtonsoft.Json;    

    public class BlobStorageProvider : IStorageProvider
    {
        private CloudBlobContainer container;

        public OrleansLogger Log { get; set; }

        public string Name { get; set; }

        public async Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
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
                var blobName = BlobStorageProvider.GetBlobName(grainType, grainId);
                var blob = container.GetBlockBlobReference(blobName);
                var text = await blob.DownloadTextAsync();
                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }

                var data = JsonConvert.DeserializeObject(text, grainState.GetType());
                var dict = ((IGrainState)data).AsDictionary();
                grainState.SetAll(dict);
            }
            catch (StorageException ex)
            {
                ;
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
                var blobName = BlobStorageProvider.GetBlobName(grainType, grainId);
                var storedData = JsonConvert.SerializeObject(grainState.AsDictionary());
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
                var blobName = BlobStorageProvider.GetBlobName(grainType, grainId);
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
