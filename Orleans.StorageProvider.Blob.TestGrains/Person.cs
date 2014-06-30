using System.Threading.Tasks;
using Orleans.StorageProvider.Blob.TestInterfaces;

namespace Orleans.StorageProvider.Blob.TestGrains
{
    [StorageProvider(ProviderName = "BlobStore")]
    public class Person : GrainBase<IPersonState>, IPerson
    {
        Task<string> IPerson.FirstName
        {
            get { return Task.FromResult(State.FirstName); }
        }

        Task<string> IPerson.LastName
        {
            get { return Task.FromResult(State.LastName); }
        }

        Task<GenderType> IPerson.Gender
        {
            get { return Task.FromResult(State.Gender); }
        }

        Task<int> IPerson.Age
        {
            get { return Task.FromResult(State.Age); }
        }

        Task IPerson.SetPersonalAttributes(PersonalAttributes props)
        {
            State.FirstName = props.FirstName;
            State.LastName = props.LastName;
            State.Gender = props.Gender;
            State.Age = props.Age;

            return State.WriteStateAsync();
        }
    }
}
