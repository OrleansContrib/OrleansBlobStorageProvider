using System.Collections.Generic;
using System.Threading.Tasks;
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
        private const string PersonId = "Orleans.StorageProvider.Blob.TestGrains.Person-00000000000000000000000000000001030000001155e6ee.json";

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

            var actualStored = await this.blobSetup.LoadAsync<DummyPersonState>(PersonId);

            actual.ShouldBeEquivalentTo(expected);
            actualStored.ShouldBeEquivalentTo(expected, options => options.ExcludingMissingProperties());
        }

        private class DummyPersonState : IPersonState
        {
            public Task ClearStateAsync()
            {
                throw new System.NotImplementedException();
            }

            public Task WriteStateAsync()
            {
                throw new System.NotImplementedException();
            }

            public Task ReadStateAsync()
            {
                throw new System.NotImplementedException();
            }

            public Dictionary<string, object> AsDictionary()
            {
                throw new System.NotImplementedException();
            }

            public void SetAll(Dictionary<string, object> values)
            {
                throw new System.NotImplementedException();
            }

            public string Etag { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public GenderType Gender { get; set; }
            public int Age { get; set; }
        }
    }
}