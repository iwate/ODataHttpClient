using Newtonsoft.Json.Linq;
using ODataHttpClient.Serializers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ODataHttpClient.Models
{
    public class RequestFactory
    {
        public IJsonSerializer JsonSerializer { get; set; }
        public RequestFactory()
            : this (Serializers.JsonSerializer.Default)
        { }
        public RequestFactory(IJsonSerializer serializer)
        {
            JsonSerializer = serializer;
        }
        public Request Create<T>(HttpMethod method, string uri, T body, IEnumerable<KeyValuePair<string, object>> additionals)
            => Request.Create<T>(method, uri, body, additionals, JsonSerializer);
        public Request Create<T>(HttpMethod method, string uri, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY)
            => Request.Create<T>(method, uri, body, type, typeKey, JsonSerializer);
        public Request Create<T>(HttpMethod method, string uri, T body, IReadOnlyDictionary<string, string> headers, IEnumerable<KeyValuePair<string, object>> additionals)
            => Request.Create<T>(method, uri, body, additionals, JsonSerializer, headers);
        public Request Create<T>(HttpMethod method, string uri, T body, IReadOnlyDictionary<string, string> headers, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY)
            => Request.Create<T>(method, uri, body, type, typeKey, JsonSerializer, headers);
            
        public Request Get(string uri) 
            => Request.Get(uri);
        
        public Request Get(string uri, object @params) 
            => Request.Get(uri, @params);

        public Request Head(string uri) 
            => Request.Head(uri);
        
        public Request Head(string uri, object @params) 
            => Request.Head(uri, @params);

        public Request Delete(string uri) 
            => Request.Delete(uri);
        
        public Request Delete(string uri, object @params) 
            => Request.Delete(uri, @params);

        public Request Post<T>(string uri, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY) 
            => Request.Post(uri, body, type, typeKey, JsonSerializer);

        public Request Post<T>(string uri, object @params, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY) 
            => Request.Post(uri, @params, body, type, typeKey, JsonSerializer);

        public Request Put<T>(string uri, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY) 
            => Request.Put(uri, body, type, typeKey, JsonSerializer);

        public Request Put<T>(string uri, object @params, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY) 
            => Request.Put(uri, @params, body, type, typeKey, JsonSerializer);

        public Request Patch<T>(string uri, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY) 
            => Request.Patch(uri, body, type, typeKey, JsonSerializer);
        
        public Request Patch<T>(string uri, object @params, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY) 
            => Request.Patch(uri, @params, body, type, typeKey, JsonSerializer);
    }
}
