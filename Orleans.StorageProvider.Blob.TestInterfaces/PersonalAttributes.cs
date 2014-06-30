using System;

namespace Orleans.StorageProvider.Blob.TestInterfaces
{
    [Serializable]
    public class PersonalAttributes
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public GenderType Gender { get; set; }
    }
}
