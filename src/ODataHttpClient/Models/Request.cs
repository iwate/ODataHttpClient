using Newtonsoft.Json.Linq;
using ODataHttpClient.Serializers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ODataHttpClient.Models
{
    public class Request : IRequest
    {
        private const string DEFAULT_TYPE_KEY = "odata.type";
        public HttpMethod Method { get; private set; }
        public string Uri { get; private set; }
        public string Body { get; private set; }

        private Request(){}
        public HttpRequestMessage CreateMessage()
        {
            var message = new HttpRequestMessage(Method, Uri);
            
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
        
        public static Request Create<T>(HttpMethod method, string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY)
        {
            var token = JToken.FromObject(body);

            if (type != null)
                token[typeKey] = type;

            return new Request
            {
                Method = method,
                Uri = uri,
                Body = JsonSerializer.Serialize(token),
            };
        }

        public static Request Get(string uri) => Create(HttpMethod.Get, uri);
        public static Request Head(string uri) => Create(HttpMethod.Head, uri);
        public static Request Delete(string uri) => Create(HttpMethod.Delete, uri);
        public static Request Post<T>(string uri, T body) => Create(HttpMethod.Post, uri, body);
        public static Request Post<T>(string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY) => Create(HttpMethod.Post, uri, body, type, typeKey);
        public static Request Put<T>(string uri, T body) => Create(HttpMethod.Put, uri, body);
        public static Request Put<T>(string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY) => Create(HttpMethod.Put, uri, body, type, typeKey);
        public static Request Patch<T>(string uri, T body) => Create(new HttpMethod("PATCH"), uri, body);
        public static Request Patch<T>(string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY) => Create(new HttpMethod("PATCH"), uri, body, type, typeKey);
    }
}
