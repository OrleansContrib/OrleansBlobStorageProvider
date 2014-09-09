using System;
using System.Threading.Tasks;

namespace Orleans.StorageProvider.Blob.Test.GrainInterfaces
{

    public interface IGrain1 : Orleans.IGrain
    {
        Task Set(string stringValue, int intValue, DateTime dateTimeValue, Guid guidValue, IGrain1 grainValue);
        Task<Tuple<string, int, DateTime, Guid, IGrain1>> Get();

    }
}
