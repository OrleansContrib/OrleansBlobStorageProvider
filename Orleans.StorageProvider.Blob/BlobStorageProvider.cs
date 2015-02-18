
namespace Orleans.StorageProvider.Blob
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Newtonsoft.Json;
    using Orleans;
    using Orleans.CodeGeneration;
    using Orleans.Providers;
    using Orleans.Runtime;
    using Orleans.Storage;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class BlobStorageProvider : IStorageProvider
    {
        private JsonSerializerSettings settings;

        private CloudBlobContainer container;

        public Logger Log { get; set; }

        public string Name { get; set; }

        public async Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            try
            {
                ConfigureJsonSerializerSettings(config);

                if (!config.Properties.ContainsKey("DataConnectionString"))
                {
                    throw new BadProviderConfigException("The DataConnectionString setting has not been configured in the cloud role. Please add a DataConnectionString setting with a valid Azure Storage connection string.");
                }
                else
                {
                    var account = CloudStorageAccount.Parse(config.Properties["DataConnectionString"]);
                    var blobClient = account.CreateCloudBlobClient();
                    var containerName = config.Properties.ContainsKey("ContainerName") ? config.Properties["ContainerName"] : "grainstate";
                    container = blobClient.GetContainerReference(containerName);
                    await container.CreateIfNotExistsAsync();
                    Log = providerRuntime.GetLogger(this.GetType().Name);
                }
            }
            catch (Exception ex)
            {
                Log.Error(0, ex.ToString());
            }
        }

        private void ConfigureJsonSerializerSettings(IProviderConfiguration config)
        {
            // By default, use automatic type name handling, simple assembly names, and no JSON formatting
            settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            settings.Formatting = Formatting.None;

            if (config.Properties.ContainsKey("SerializeTypeNames"))
            {
                bool serializeTypeNames = false;
                var serializeTypeNamesValue = config.Properties["SerializeTypeNames"];
                bool.TryParse(serializeTypeNamesValue, out serializeTypeNames);
                if (serializeTypeNames)
                {
                    settings.TypeNameHandling = TypeNameHandling.All;
                }
            }

            if (config.Properties.ContainsKey("UseFullAssemblyNames"))
            {
                bool useFullAssemblyNames = false;
                var UseFullAssemblyNamesValue = config.Properties["UseFullAssemblyNames"];
                bool.TryParse(UseFullAssemblyNamesValue, out useFullAssemblyNames);
                if (useFullAssemblyNames)
                {
                    settings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full;
                }
            }

            if (config.Properties.ContainsKey("IndentJSON"))
            {
                bool indentJSON = false;
                var indentJSONValue = config.Properties["IndentJSON"];
                bool.TryParse(indentJSONValue, out indentJSON);
                if (indentJSON)
                {
                    settings.Formatting = Formatting.Indented;
                }
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

                var exists = await blob.ExistsAsync();
                if (!exists)
                {
                    return;
                }

                var text = await blob.DownloadTextAsync();
                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }

                var data = JsonConvert.DeserializeObject(text, grainState.GetType(), settings);
                var dict = ((IGrainState)data).AsDictionary();
                grainState.SetAll(dict);
                grainState.Etag = blob.Properties.ETag;
            }
            catch (Exception ex)
            {
                Log.Error(0, ex.ToString());
            }
        }

        private static string GetBlobName(string grainType, GrainReference grainId)
        {
            return string.Format("{0}-{1}.json", grainType, grainId.ToKeyString());
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainId, IGrainState grainState)
        {
            try
            {
                var blobName = BlobStorageProvider.GetBlobName(grainType, grainId);
                var grainStateDictionary = grainState.AsDictionary();
                var storedData = JsonConvert.SerializeObject(grainStateDictionary, settings);
                Log.Verbose("Serialized grain state is: {0}.", storedData);

                var blob = container.GetBlockBlobReference(blobName);
                await
                    blob.UploadTextAsync(
                        storedData,
                        Encoding.UTF8,
                        AccessCondition.GenerateIfMatchCondition(grainState.Etag),
                        null,
                        null);
                grainState.Etag = blob.Properties.ETag;
            }
            catch (Exception ex)
            {
                Log.Error(0, ex.ToString());
            }
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainId, IGrainState grainState)
        {
            try
            {
                var blobName = BlobStorageProvider.GetBlobName(grainType, grainId);
                var blob = container.GetBlockBlobReference(blobName);
                await
                    blob.DeleteIfExistsAsync(
                        DeleteSnapshotsOption.None,
                        AccessCondition.GenerateIfMatchCondition(grainState.Etag),
                        null,
                        null);
                grainState.Etag = blob.Properties.ETag;
            }
            catch (Exception ex)
            {
                Log.Error(0, ex.ToString());
            }
        }
    }
}
