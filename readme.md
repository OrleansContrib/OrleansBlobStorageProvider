# Orleans Blob Storage Provider

Stores Orleans grain state in blob storage as Json.

## Installation

The Orleans SDK must be installed first.

Build the project. This will copy OrleansBlobStorageProvider.dll and required reference to the Orleans SDK directory.

```xml
<?xml version="1.0" encoding="utf-8"?>
<OrleansConfiguration xmlns="urn:orleans">
  <Globals>
    <StorageProviders>
      <Provider Type="OrleansBlobStorageProvider.BlobStorageProvider" Name="BlobStore" DataConnectionString="UseDevelopmentStorage=true" ContainerName="grainstate"/>
    </StorageProviders>
    ...
```

Then from your grain code configure grain storage:

```cs
// define a state interface
public interface IMyGrainState : IGrainState
{
    string Value { get; set; }
}

// Select the BlobStore as the storage provider for the grain
[StorageProvider(ProviderName="BlobStore")]
public class Grain1 : Orleans.GrainBase<IMyGrainState>, IGrain1
{
    public Task Test(string value)
    {
    	// set the state and save it
        this.State.Value = value;
        return this.State.WriteStateAsync();
    }

}
```

Grains are stored in json format with the following convention for blob names:

```
{GrainType}-{GrainId}.json
```

i.e.

```
Grain1-0.json
```

## License

MIT