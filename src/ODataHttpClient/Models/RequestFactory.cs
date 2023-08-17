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
        public Request Create(HttpMethod method, string uri, bool acceptNotFound = false)
            => Request.Create(method, uri, acceptNotFound);
        public Request Create(HttpMethod method, string uri, IReadOnlyDictionary<string, string> headers, bool acceptNotFound = false)
            => Request.Create(method, uri, headers, acceptNotFound);
        public Request Create<T>(HttpMethod method, string uri, T body, string type = null, string typeKey = Request.DEFAULT_TYPE_KEY, IReadOnlyDictionary<string, string> headers = null, bool acceptNotFound = false)
            => Request.Create<T>(method, uri, body, type, typeKey, JsonSerializer, headers, acceptNotFound);
        public Request Create<T>(HttpMethod method, string uri, T body, IEnumerable<KeyValuePair<string, object>> additionals, IReadOnlyDictionary<string, string> headers = null, bool acceptNotFound = false)
            => Request.Create<T>(method, uri, body, additionals, JsonSerializer, headers, acceptNotFound);
            
        public Request Get(string uri, bool acceptNotFound = true) 
            => Request.Get(uri, acceptNotFound);
        
        public Request Get(string uri, object @params, bool acceptNotFound = true) 
            => Request.Get(uri, @params, acceptNotFound);

        public Request Head(string uri, bool acceptNotFound = true) 
            => Request.Head(uri, acceptNotFound);
        
        public Request Head(string uri, object @params, bool acceptNotFound = true) 
            => Request.Head(uri, @params, acceptNotFound);

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
