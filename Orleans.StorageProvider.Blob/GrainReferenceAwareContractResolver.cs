namespace OrleansBlobStorageProvider
{
    using System;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Orleans;

    internal class GrainReferenceAwareContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            if (typeof(IGrain).IsAssignableFrom(objectType))
            {
                contract.Converter = new GrainReferenceConverter();
            }

            return contract;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = instance => !typeof(GrainReference).IsAssignableFrom(property.DeclaringType);
            return property;
        }
    }
}