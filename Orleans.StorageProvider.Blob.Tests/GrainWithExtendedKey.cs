using NUnit.Framework;
using Orleans.StorageProvider.Blob.TestGrains;
using Orleans.StorageProvider.Blob.TestInterfaces;
using Orleans.StorageProvider.Blob.Tests.Infrastructure;

namespace Orleans.StorageProvider.Blob.Tests
{
    [TestFixture]
    public class GrainWithExtendedKey
    {
        private const string EmailId = "EmailState/0000000000000000000000000000000106ffffffc4b12014+asdf@gmail.bs";

        private BlobSetup blobSetup;
        private SiloSetup siloSetup;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            this.blobSetup = new BlobSetup();
            this.siloSetup = new SiloSetup();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            this.siloSetup.Dispose();
            this.blobSetup.Dispose();
        }

        [Test]
        public async void LoadedSuccessful()
        {
            var email = EmailFactory.GetGrain(1, "asdf@gmail.bs");
            await email.Send();

            //IAsyncDocumentSession session = this.blobSetup.NewAsyncSession();
            //var actualStored = await session.LoadAsync<IEmailState>(EmailId);

            //actualStored.Email.Should().Be("asdf@gmail.bs");
            //actualStored.SentAt.Should().HaveValue();
            //actualStored.Person.Should().BeNull();
        }
    }
}