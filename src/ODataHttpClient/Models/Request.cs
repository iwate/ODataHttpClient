using Newtonsoft.Json.Linq;
using ODataHttpClient.Serializers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using JsonSettings = Newtonsoft.Json.JsonSerializerSettings;

namespace ODataHttpClient.Models
{
    public class Request : IRequest
    {
        internal const string DEFAULT_TYPE_KEY = "odata.type";
        private static readonly Func<string, string, Action<JToken>> _setOdataType = (key, val) => token => { if (val != null) token[key] = val; };
        public HttpMethod Method { get; private set; }
        public string Uri { get; private set; }
        public string Body { get; private set; }

        private Request(){}
        public HttpRequestMessage CreateMessage()
        {
            var message = new HttpRequestMessage(Method, Uri);
            
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            
            if (Body != null)
                message.Content = new StringContent(Body, Encoding.UTF8, "application/json");

            return message;
        }

        public static Request Create(HttpMethod method, string uri)
        {
            return new Request
            {
                Method = method,
                Uri = uri,
                Body = null,
            };
        }
        public static Request Create<T>(HttpMethod method, string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, JsonSettings jsonSettings = null)
        {
            return Create(method, uri, body, _setOdataType(typeKey, type), jsonSettings ?? JsonSerializer.DefaultJsonSerializerSettings);
        }

        public static Request Create<T>(HttpMethod method, string uri, T body, Action<JToken> builder, JsonSettings jsonSettings)
        {
            var token = JToken.FromObject(body);

            builder?.Invoke(token);

            return new Request
            {
                Method = method,
                Uri = uri,
                Body = JsonSerializer.Serialize(token, jsonSettings),
            };
        }

        public static Request Get(string uri) => Create(HttpMethod.Get, uri);

        public static Request Head(string uri) => Create(HttpMethod.Head, uri);

        public static Request Delete(string uri) => Create(HttpMethod.Delete, uri);

        public static Request Post<T>(string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, JsonSettings jsonSettings = null) 
            => Create(HttpMethod.Post, uri, body, type, typeKey, jsonSettings);

        public static Request Put<T>(string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, JsonSettings jsonSettings = null) 
            => Create(HttpMethod.Put, uri, body, type, typeKey, jsonSettings);

        public static Request Patch<T>(string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, JsonSettings jsonSettings = null) 
            => Create(new HttpMethod("PATCH"), uri, body, type, typeKey, jsonSettings);
    }
}
