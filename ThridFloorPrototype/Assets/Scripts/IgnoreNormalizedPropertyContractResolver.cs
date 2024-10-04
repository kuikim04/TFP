using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreNormalizedPropertyContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // Ignore the 'normalized' property in Vector3
        if (property.DeclaringType == typeof(UnityEngine.Vector3) && property.PropertyName == "normalized")
        {
            property.ShouldSerialize = instance => false;
        }

        return property;
    }
}
