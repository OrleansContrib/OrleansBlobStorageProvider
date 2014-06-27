using System;
using Orleans.StorageProvider.Blob.TestInterfaces;

namespace Orleans.StorageProvider.Blob.TestGrains
{
    public interface IEmailState : IGrainState
    {
        string Email { get; set; }
        DateTimeOffset? SentAt { get; set; }
        IPerson Person { get; set; }
    }
}