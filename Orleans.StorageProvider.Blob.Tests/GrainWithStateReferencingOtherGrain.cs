using FluentAssertions;
using NUnit.Framework;
using Orleans.StorageProvider.Blob.TestInterfaces;
using Orleans.StorageProvider.Blob.Tests.Infrastructure;

namespace Orleans.StorageProvider.Blob.Tests
{
    [TestFixture]
    public class GrainWithStateReferencingOtherGrain
    {
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
            var person = PersonFactory.GetGrain(2);
            await person.SetPersonalAttributes(new PersonalAttributes { FirstName = "John Copy", LastName = "Doe Copy", Age = 24, Gender = GenderType.Male });

            var email = EmailFactory.GetGrain(2, "asdf@gmail.bs");
            await email.SetPerson(person);
            await email.Send();

            var emailAgain = EmailFactory.GetGrain(2, "asdf@gmail.bs");
            var expectedPerson = await emailAgain.Person;

            expectedPerson.Should().NotBeNull();
        }
    }
}