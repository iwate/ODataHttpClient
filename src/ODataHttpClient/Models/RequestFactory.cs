using Newtonsoft.Json.Linq;
using ODataHttpClient.Serializers;
using System;
using System.Net.Http;
using JsonSettings = Newtonsoft.Json.JsonSerializerSettings;
namespace ODataHttpClient.Models
{
    public class RequestFactory
    {
        public JsonSettings JsonSerializerSettings { get; set; }
        public RequestFactory()
            : this (JsonSerializer.DefaultJsonSerializerSettings)
        { }
        public RequestFactory(JsonSettings jsonSettings)
        {
            JsonSerializerSettings = jsonSettings;
        }
        public Request Create<T>(HttpMethod method, string uri, T body, Action<JToken> builder)
            => Request.Create<T>(method, uri, body, builder, JsonSerializerSettings);
        public Request Create<T>(HttpMethod method, string uri, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY)
            => Request.Create<T>(method, uri, body, type, typeKey, JsonSerializerSettings);

        public Request Get(string uri) 
            => Request.Get(uri);

        public Request Head(string uri) 
            => Request.Head(uri);

        public Request Delete(string uri) 
            => Request.Delete(uri);

        public Request Post<T>(string uri, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY) 
            => Request.Post(uri, body, type, typeKey, JsonSerializerSettings);

        public Request Put<T>(string uri, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY) 
            => Request.Put(uri, body, type, typeKey, JsonSerializerSettings);

        public Request Patch<T>(string uri, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY) 
            => Request.Patch(uri, body, type, typeKey, JsonSerializerSettings);
    }
}
