using System;
using Newtonsoft.Json;

namespace ODataHttpClient.Serializers
{
    public class ByteArrayConverter : JsonConverter
    {
        private static Type _targetType = typeof(byte[]);
        public override bool CanConvert(Type objectType)
        {
            return objectType == _targetType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            
            var base64 = (string)serializer.Deserialize(reader);

            return Convert.FromBase64String(base64);
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var base64 = Convert.ToBase64String((byte[])value);

            serializer.Serialize(writer, base64);
        }
    }
}