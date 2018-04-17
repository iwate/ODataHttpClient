using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ODataHttpClient.Serializers
{
    public class JsonSerializer
    {
        public static JsonSerializerSettings HistoricalJsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = 
            {
                new TypeToStringConverter<long>(),
                new TypeToStringConverter<long?>(),
                new TypeToStringConverter<double>(),
                new TypeToStringConverter<double?>(),
                new TypeToStringConverter<decimal>(),
                new TypeToStringConverter<decimal?>(),
                new ByteArrayConverter(),
            }
        };
        public static JsonSerializerSettings GeneralJsonSerializerSettings = new JsonSerializerSettings
        {
            Converters =
            {
                new ByteArrayConverter(),
            }
        };
        public static JsonSerializerSettings DefaultJsonSerializerSettings = HistoricalJsonSerializerSettings;

        public static string Serialize<T>(T obj)
        {
            return Serialize(obj, DefaultJsonSerializerSettings);
        }
        public static string Serialize<T>(T obj, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(obj, DefaultJsonSerializerSettings);
        }
        public static string Serialize(JToken token)
        {
            return Serialize(token, DefaultJsonSerializerSettings);
        }
        public static string Serialize(JToken token, JsonSerializerSettings settings)
        {
            return token.ToString(settings.Formatting, settings.Converters.ToArray());
        }
        public static T Deserialize<T>(string json)
        {
            return Deserialize<T>(json, DefaultJsonSerializerSettings);
        }
        public static T Deserialize<T>(string json, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
        public static T DeserializeAt<T>(string json, string path)
        {
            return DeserializeAt<T>(json, path, DefaultJsonSerializerSettings);
        }
        public static T DeserializeAt<T>(string json, string path, JsonSerializerSettings settings)
        {
            var serializer = Newtonsoft.Json.JsonSerializer.Create(DefaultJsonSerializerSettings);
            return JToken.Parse(json).SelectToken(path).ToObject<T>(serializer);
        }
    }
}
