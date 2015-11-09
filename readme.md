[![Build Status](https://travis-ci.org/OrleansContrib/OrleansBlobStorageProvider.svg?branch=master)](https://travis-ci.org/OrleansContrib/OrleansBlobStorageProvider)

# Orleans Blob Storage Provider

Stores Orleans grain state in blob storage as Json.

The `master` branch targets the latest release.

The `April_14` branch targets the initial release.

## Installation

The Orleans SDK must be installed first.

Using NuGet, with the GrainClasses (or Silo Host) project as your target:

```
Install-Package Orleans.StorageProvider.Blob
```

Then register the provider in your Silo Configuration:

```xml
<?xml version="1.0" encoding="utf-8"?>
<OrleansConfiguration xmlns="urn:orleans">
  <Globals>
    <StorageProviders>
      <Provider Type="Orleans.StorageProvider.Blob.BlobStorageProvider" Name="BlobStore" DataConnectionString="UseDevelopmentStorage=true" ContainerName="grainstate"/>
    </StorageProviders>
    ...
```

Optional Attributes that can be added to the provider element
* PreserveReferencesHandling="true" - Preserves reference handling for objects during Json serialization

Then from your grain code configure grain storage:

```cs
// define a state interface
public interface IMyGrainState : IGrainState
{
    string Value { get; set; }
}

// Select the BlobStore as the storage provider for the grain
[StorageProvider(ProviderName="BlobStore")]
public class Grain1 : Orleans.Grain<IMyGrainState>, IGrain1
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
GrainCollection1.Grain1-0.json
```

## License

MIT
