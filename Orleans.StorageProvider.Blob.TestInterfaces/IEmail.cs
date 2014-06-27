using System.Threading.Tasks;

namespace Orleans.StorageProvider.Blob.TestInterfaces
{
    [ExtendedPrimaryKey]
    public interface IEmail : IGrain
    {
        Task SetPerson(IPerson person);
        Task<IPerson> Person { get; }
        Task Send();
    }
}