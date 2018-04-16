using ODataHttpClient.Serializers;
using System;
using System.IO;
using System.Net;
using System.Text;
using JsonSettings = Newtonsoft.Json.JsonSerializerSettings;

namespace ODataHttpClient.Models
{
    public class Response
    {
        private JsonSettings _jsonSettings;
        public bool Success { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string MediaType { get; private set; }
        public string ErrorMessage { get; private set; }
        public byte[] Binary { get; private set; }
        public string Body { get => Binary != null ? Encoding.UTF8.GetString(Binary) : null; }
        private Response(){}

        public T ReadAs<T>(string jsonPath = null)
        {
            return ReadAs<T>(jsonPath, _jsonSettings);
        }
        public T ReadAs<T>(JsonSettings jsonSettings)
        {
            return ReadAs<T>(null, jsonSettings);
        }
        public T ReadAs<T>(string jsonPath, JsonSettings jsonSettings)
        {
            if (Body == null)
                return default(T);

            if (MediaType == "application/json")
            {
                if (jsonPath == null)
                    return JsonSerializer.Deserialize<T>(Body, jsonSettings);

                else
                    return JsonSerializer.DeserializeAt<T>(Body, jsonPath, jsonSettings);
            }

            var type = typeof(T);

            if (MediaType == "text/plain")
            {
                if (type == typeof(string))
                    return (T)(object)Body;

                if (type == typeof(int) || type == typeof(int?))
                    return (T)(object)Convert.ToInt32(Body);

                if (type == typeof(long) || type == typeof(long?))
                    return (T)(object)Convert.ToInt64(Body);

                if (type == typeof(double) || type == typeof(double?))
                    return (T)(object)Convert.ToDouble(Body);

                if (type == typeof(decimal) || type == typeof(decimal?))
                    return (T)(object)Convert.ToDecimal(Body);

                if (type == typeof(DateTime) || type == typeof(DateTime?))
                    return (T)(object)Convert.ToDateTime(Body);

                if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
                    return (T)(object)DateTimeOffset.Parse(Body);
            }

            if (MediaType == "application/octet-stream")
            {
                if (type == typeof(byte[]))
                    return (T)(object)Binary;

                if (type == typeof(Stream))
                    return (T)(object)new MemoryStream(Binary);
            }

            throw new NotSupportedException();
        }

        public static Response CreateError(HttpStatusCode code, byte[] body)
        {
            return CreateError(code, body != null ? Encoding.UTF8.GetString(body) : null);
        }
        public static Response CreateError(HttpStatusCode code, string message)
        {
            return new Response { Success = false, StatusCode = code, ErrorMessage = message };
        }

        public static Response CreateSuccess(HttpStatusCode code, string mime, string body)
        {
            return CreateSuccess(code, mime, body, JsonSerializer.DefaultJsonSerializerSettings);
        }
        public static Response CreateSuccess(HttpStatusCode code, string mime, byte[] body)
        {
            return CreateSuccess(code, mime, body, JsonSerializer.DefaultJsonSerializerSettings);
        }
        public static Response CreateSuccess(HttpStatusCode code, string mime, string body, JsonSettings jsonSettings)
        {
            return CreateSuccess(code, mime, body != null ? Encoding.UTF8.GetBytes(body) : null, jsonSettings);
        }
        public static Response CreateSuccess(HttpStatusCode code, string mime, byte[] body, JsonSettings jsonSettings)
        {
            return new Response { Success = true, StatusCode = code, MediaType = mime, Binary = body, _jsonSettings = jsonSettings };
        }
    }
}
