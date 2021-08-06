using ODataHttpClient.Serializers;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ODataHttpClient.Models
{
    public class Response
    {
        private IJsonSerializer _serializer = JsonSerializer.Default;
        public bool Success { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string MediaType { get; private set; }
        public string ErrorMessage { get; private set; }
        public byte[] Binary { get; private set; }
        public string Body { get => GetStringFromUTF8(Binary); }
        public HttpResponseHeaders Headers { get; private set; }

        private Response() { }

        public T ReadAs<T>(string jsonPath = null)
        {
            return ReadAs<T>(jsonPath, _serializer);
        }
        public T ReadAs<T>(IJsonSerializer serializer)
        {
            return ReadAs<T>(null, serializer);
        }
        public T ReadAs<T>(string jsonPath, IJsonSerializer serializer)
        {
            if (Body == null)
                return default(T);

            if (MediaType == "application/json")
            {
                if (jsonPath == null)
                    return serializer.Deserialize<T>(Body);

                else
                    return serializer.Deserialize<T>(Body, jsonPath);
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
                
                if (type == typeof(float) || type == typeof(float?))
                    return (T)(object)Convert.ToSingle(Body, CultureInfo.InvariantCulture);

                if (type == typeof(double) || type == typeof(double?))
                    return (T)(object)Convert.ToDouble(Body, CultureInfo.InvariantCulture);

                if (type == typeof(decimal) || type == typeof(decimal?))
                    return (T)(object)Convert.ToDecimal(Body, CultureInfo.InvariantCulture);

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

        private static readonly byte[] _utf8Preamble = Encoding.UTF8.GetPreamble();
        private static bool IsUTF8WithBOM(byte[] binary) => binary.Take(_utf8Preamble.Length).SequenceEqual(_utf8Preamble);
        private static (int start, int len) GetUTF8StartAndLength(byte[] binary) 
        {
            if (IsUTF8WithBOM(binary))
                return (_utf8Preamble.Length, binary.Length - _utf8Preamble.Length);
                
            return (0, binary.Length);
        } 
        public static string GetStringFromUTF8(byte[] binary) 
        {
            if (binary == null)
                return null;
            
            var (start, len) = GetUTF8StartAndLength(binary);
            return Encoding.UTF8.GetString(binary, start, len);
        }

        public static Response CreateError(HttpStatusCode code, byte[] body, HttpResponseHeaders headers)
        {
            return CreateError(code, GetStringFromUTF8(body), headers);
        }
        public static Response CreateError(HttpStatusCode code, string message, HttpResponseHeaders headers)
        {
            return new Response { Success = false, StatusCode = code, ErrorMessage = message, Headers = headers };
        }

        public static Response CreateSuccess(HttpStatusCode code, string mime, string body, HttpResponseHeaders headers = null)
        {
            return CreateSuccess(code, mime, body, JsonSerializer.Default, headers);
        }
        public static Response CreateSuccess(HttpStatusCode code, string mime, byte[] body, HttpResponseHeaders headers = null)
        {
            return CreateSuccess(code, mime, body, JsonSerializer.Default, headers);
        }
        public static Response CreateSuccess(HttpStatusCode code, string mime, string body, IJsonSerializer serializer, HttpResponseHeaders headers)
        {
            return CreateSuccess(code, mime, body != null ? Encoding.UTF8.GetBytes(body) : null, serializer, headers);
        }
        public static Response CreateSuccess(HttpStatusCode code, string mime, byte[] body, IJsonSerializer serializer, HttpResponseHeaders headers)
        {
            return new Response { Success = true, StatusCode = code, MediaType = mime, Binary = body, _serializer = serializer, Headers = headers };
        }
    }
}
