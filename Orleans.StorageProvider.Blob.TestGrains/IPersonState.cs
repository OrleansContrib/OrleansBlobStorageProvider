using Orleans.StorageProvider.Blob.TestInterfaces;

namespace Orleans.StorageProvider.Blob.TestGrains
{
    public interface IPersonState : IGrainState
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        GenderType Gender { get; set; }
        int Age { get; set; }
    }
}