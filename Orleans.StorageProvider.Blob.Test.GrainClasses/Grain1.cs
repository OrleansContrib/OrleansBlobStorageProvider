using Orleans.Providers;
using Orleans.StorageProvider.Blob.Test.GrainInterfaces;
using System;
using System.Threading.Tasks;

namespace Orleans.StorageProvider.Blob.Test.GrainClasses
{

    public interface MyState : IGrainState
    {
        string StringValue { get; set; }
        int IntValue { get; set; }
        DateTime DateTimeValue { get; set; }
        Guid GuidValue { get; set; }
        IGrain1 GrainValue { get; set; }
    }

    [StorageProvider(ProviderName = "JSON")]
    public class Grain1 : Orleans.Grain<MyState>, IGrain1
    {
        public Task Set(string stringValue, int intValue, DateTime dateTimeValue, Guid guidValue, IGrain1 grainValue)
        {
            this.State.StringValue = stringValue;
            this.State.IntValue = intValue;
            this.State.DateTimeValue = dateTimeValue;
            this.State.GuidValue = guidValue;
            this.State.GrainValue = grainValue;
            return this.State.WriteStateAsync();
        }

        public Task<Tuple<string, int, DateTime, Guid, IGrain1>> Get()
        {
            return Task.FromResult(new Tuple<string, int, DateTime, Guid, IGrain1>(
                this.State.StringValue,
                this.State.IntValue,
                this.State.DateTimeValue,
                this.State.GuidValue,
                this.State.GrainValue));

        }
    }
}
