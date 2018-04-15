using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ODataHttpClient.Serializers
{
    public class TypeToStringConverter<T> : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JToken jt = JValue.ReadFrom(reader);

            return jt.Value<T>();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).Equals(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            serializer.Serialize(writer, value?.ToString());
        }
    }
}
