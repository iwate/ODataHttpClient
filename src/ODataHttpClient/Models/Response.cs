using ODataHttpClient.Serializers;
using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ODataHttpClient.Models
{
    public class Response : IResponse
    {
        private IJsonSerializer _serializer = JsonSerializer.Default;
        public bool Success { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string MediaType { get; private set; }
        public string ErrorMessage { get; private set; }
        public byte[] Binary { get; private set; }
        public string Body { get => Binary != null ? Encoding.UTF8.GetString(Binary) : null; }
		public HttpRequestHeaders Headers { get; private set; }

		private Response(){}

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

        public static Response CreateError(HttpStatusCode code, byte[] body, HttpRequestHeaders headers)
        {
            return CreateError(code, body != null ? Encoding.UTF8.GetString(body) : null, headers);
        }
        public static Response CreateError(HttpStatusCode code, string message, HttpRequestHeaders headers)
        {
            return new Response { Success = false, StatusCode = code, ErrorMessage = message, Headers = headers };
        }

        public static Response CreateSuccess(HttpStatusCode code, string mime, string body, HttpRequestHeaders headers = null)
        {
            return CreateSuccess(code, mime, body, JsonSerializer.Default, headers);
        }
        public static Response CreateSuccess(HttpStatusCode code, string mime, byte[] body, HttpRequestHeaders headers = null)
        {
            return CreateSuccess(code, mime, body, JsonSerializer.Default, headers);
        }
        public static Response CreateSuccess(HttpStatusCode code, string mime, string body, IJsonSerializer serializer, HttpRequestHeaders headers)
        {
            return CreateSuccess(code, mime, body != null ? Encoding.UTF8.GetBytes(body) : null, serializer, headers);
        }
        public static Response CreateSuccess(HttpStatusCode code, string mime, byte[] body, IJsonSerializer serializer, HttpRequestHeaders headers)
        {
            return new Response { Success = true, StatusCode = code, MediaType = mime, Binary = body, _serializer = serializer, Headers = headers };
        }
	}
}
