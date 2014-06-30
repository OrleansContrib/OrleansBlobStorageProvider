using System.Threading.Tasks;

namespace Orleans.StorageProvider.Blob.TestInterfaces
{
    /// <summary>
    /// Orleans grain communication interface IPerson
    /// </summary>
    public interface IPerson : IGrain
    {
        Task SetPersonalAttributes(PersonalAttributes person);

        Task<string> FirstName { get; }
        Task<string> LastName { get; }
        Task<int> Age { get; }
        Task<GenderType> Gender { get; }
    }
}