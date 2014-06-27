using System;
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
    public class GrainWithExtendedKey
    {
        private const string EmailId = "Orleans.StorageProvider.Blob.TestGrains.Email-0000000000000000000000000000000106ffffffa4de1a69+asdf@gmail.bs.json";

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

            var actualStored = await this.blobSetup.LoadAsync<DummyEmailState>(EmailId);

            actualStored.Email.Should().Be("asdf@gmail.bs");
            actualStored.SentAt.Should().HaveValue();
            actualStored.Person.Should().BeNull();
        }

        private class DummyEmailState : IEmailState
        {
            public Task ClearStateAsync()
            {
                throw new NotImplementedException();
            }

            public Task WriteStateAsync()
            {
                throw new NotImplementedException();
            }

            public Task ReadStateAsync()
            {
                throw new NotImplementedException();
            }

            public Dictionary<string, object> AsDictionary()
            {
                throw new NotImplementedException();
            }

            public void SetAll(Dictionary<string, object> values)
            {
                throw new NotImplementedException();
            }

            public string Etag { get; set; }
            public string Email { get; set; }
            public DateTimeOffset? SentAt { get; set; }
            public IPerson Person { get; set; }
        }
    }
}