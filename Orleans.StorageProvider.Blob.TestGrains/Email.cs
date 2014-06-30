using System;
using System.Threading.Tasks;
using Orleans.StorageProvider.Blob.TestInterfaces;

namespace Orleans.StorageProvider.Blob.TestGrains
{
    [StorageProvider(ProviderName = "BlobStore")]
    public class Email : GrainBase<IEmailState>, IEmail
    {
        private string email;

        public Task<IPerson> Person
        {
            get { return Task.FromResult(State.Person); }
        }


        public Task<DateTimeOffset?> SentAt
        {
            get { return Task.FromResult(State.SentAt); }
        }

        public async Task Send()
        {
            if (this.State.SentAt.HasValue)
            {
                return;
            }

            this.State.SentAt = DateTimeOffset.UtcNow;

            await this.State.WriteStateAsync();
        }

        public async Task SetPerson(IPerson person)
        {
            State.Person = person;

            await this.State.WriteStateAsync();
        }

        public override Task ActivateAsync()
        {
            this.GetPrimaryKey(out this.email);
            this.State.Email = this.email;

            return base.ActivateAsync();
        }
    }
}