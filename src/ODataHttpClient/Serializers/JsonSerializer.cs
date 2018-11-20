using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private static readonly Type _this = typeof(JsonSerializer);
        private static readonly MethodInfo _deserializeArray = _this.GetMethod(nameof(DeserializeArray),BindingFlags.NonPublic|BindingFlags.Instance);
        public T Deserialize<T>(string json, string path)
        {
            if (string.IsNullOrEmpty(path))
                return Deserialize<T>(json);

            var type = typeof(T);

            if (!type.IsMultiple())
                return DeserializeObject<T>(json, path);

            // This code cannot cover all jsonpath pattern.
            // If you have faced trouble, make a issue, please:)
            if (!path.Contains("..") && !path.Contains("["))
                return DeserializeObject<T>(json, path);

            var itemType = type.GetItemType();

            if (itemType == null)
                return DeserializeObject<T>(json, path);
            
            var method = _deserializeArray.MakeGenericMethod(type, itemType);
            return (T)method.Invoke(this, new []{json, path});
        }

        private T DeserializeObject<T>(string json, string path)
        {
            var token = JToken.Parse(json).SelectToken(path);
            
            if (token == null)
                return default(T);

            return token.ToObject<T>(_serializer);
        }

        private T DeserializeArray<T, E>(string json, string path) where T : class, IEnumerable<E>
        {
            var items = JToken.Parse(json).SelectTokens(path).Select(t => t.ToObject<E>(_serializer));
            var type = typeof(T);
            var fullName = type.FullName;

            if (type.IsArray || fullName.StartsWith("System.Collections.Generic.IEnumerable"))
                return items.ToArray() as T;
            
            if (fullName.StartsWith("System.Collections.Generic.ICollection") || 
                fullName.StartsWith("System.Collections.Generic.List"))
                return items.ToList() as T;

            throw new NotSupportedException($"{type.FullName} is not supported now.");
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
