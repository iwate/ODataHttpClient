using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ODataHttpClient.Serializers
{
    public class JsonSerializer : IJsonSerializer
    {
        private JsonSerializerSettings _settings;
        private Newtonsoft.Json.JsonSerializer _serializer;

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public T Deserialize<T>(string json, string path)
        {
            var token = JToken.Parse(json).SelectToken(path);
            
            if (token == null)
                return default(T);

            return token.ToObject<T>(_serializer);
        }

        private static readonly JsonSerializerSettings _historical = new JsonSerializerSettings
        {
            Converters =
            {
                new ByteArrayConverter(),
                new TypeToStringConverter<long>(),
                new TypeToStringConverter<long?>(),
                new TypeToStringConverter<double>(),
                new TypeToStringConverter<double?>(),
                new TypeToStringConverter<decimal>(),
                new TypeToStringConverter<decimal?>(),
            }
        };
        private static readonly JsonSerializerSettings _general = new JsonSerializerSettings
        {
            Converters =
            {
                new ByteArrayConverter()
            }
        };
        public static JsonSerializer Historical = new JsonSerializer
        {
            _settings = _historical,
            _serializer = Newtonsoft.Json.JsonSerializer.Create(_historical)
        };
        public static JsonSerializer General = new JsonSerializer
        {
            _settings = _general,
            _serializer = Newtonsoft.Json.JsonSerializer.Create(_general)
        };
        public static IJsonSerializer Default = Historical;
    }
}
