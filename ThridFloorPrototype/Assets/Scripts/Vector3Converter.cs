using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Converter : JsonConverter<Vector3>
{
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        JObject obj = new JObject(
            new JProperty("x", value.x),
            new JProperty("y", value.y),
            new JProperty("z", value.z)
        );
        obj.WriteTo(writer);
    }

    public override Vector3 ReadJson(JsonReader reader, System.Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new System.NotImplementedException();
    }

    public override bool CanRead => false;

    public override bool CanWrite => true;
}
