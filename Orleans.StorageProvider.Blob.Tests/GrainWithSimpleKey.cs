using FluentAssertions;
using NUnit.Framework;
using Orleans.StorageProvider.Blob.TestGrains;
using Orleans.StorageProvider.Blob.TestInterfaces;
using Orleans.StorageProvider.Blob.Tests.Infrastructure;

namespace Orleans.StorageProvider.Blob.Tests
{
    [TestFixture]
    public class GrainWithSimpleKey
    {
        private const string PersonId = "PersonState/0000000000000000000000000000000103ffffffd411bc7a";

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
            var expected = new PersonalAttributes { FirstName = "John Copy", LastName = "Doe Copy", Age = 24, Gender = GenderType.Male };

            var person = PersonFactory.GetGrain(1);
            await person.SetPersonalAttributes(expected);

            var result = PersonFactory.GetGrain(1);
            var actual = new PersonalAttributes
            {
                FirstName = await result.FirstName,
                LastName = await result.LastName,
                Age = await result.Age,
                Gender = await result.Gender
            };

            //IAsyncDocumentSession session = this.blobSetup.NewAsyncSession();
            //var actualStored = await session.LoadAsync<IPersonState>(PersonId);

            //actual.ShouldBeEquivalentTo(expected);
            //actualStored.ShouldBeEquivalentTo(expected, options => options.ExcludingMissingProperties());
        }
    }
}