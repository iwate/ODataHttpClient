using Newtonsoft.Json.Linq;
using ODataHttpClient.Parameterizers;
using ODataHttpClient.Serializers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ODataHttpClient.Models
{
	public class Request : IRequest
    {
        internal const string DEFAULT_TYPE_KEY = "odata.type";
        private static Type stringType = typeof(string);
        public HttpMethod Method { get; private set; }
		public HttpRequestHeaders Headers { get; private set; }
		public string Uri { get; private set; }
        public string MediaType { get; private set; }
        public string Body { get; private set; }

        private Request(){}
        
        public HttpRequestMessage CreateMessage()
        {
	        var message = new HttpRequestMessage(Method, Uri);
	        message.Version = new Version(1, 1);
	        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
	        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

	        if (Body != null)
		        message.Content = new StringContent(Body, Encoding.UTF8, MediaType);
 
	        return message;
        }

		public static Request Create(HttpMethod method, string uri)
		{
			return new Request
			{
				Method = method,
				Uri = uri,
				Body = null
			};
		}

		public static Request Create(HttpMethod method, string uri, HttpRequestHeaders headers)
        {
            return new Request
            {
                Method = method,
                Uri = uri,
                Body = null,
				Headers = headers
            };
        }

        public static Request Create<T>(HttpMethod method, string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null, HttpRequestHeaders headers = null)
        {
            return Create(method, uri, body, type != null ? new[] { new KeyValuePair<string, object>(typeKey, type) } : null, serializer ?? JsonSerializer.Default, headers);
        }

		public static Request Create<T>(HttpMethod method, string uri, T body, IEnumerable<KeyValuePair<string, object>> additionals, IJsonSerializer serializer, HttpRequestHeaders headers = null)
        {
			string content, mime = null;
            
            var type = typeof(T);

            if (type.IsValueType || type == stringType)
            {
                content = body.ToString();
                mime = "text/plain";
            }
            else
            {
                var json = JObject.FromObject(body);

                foreach (var additional in additionals ?? new KeyValuePair<string, object>[0])
                    json.Add(additional.Key, JToken.FromObject(additional.Value));

                content = serializer.Serialize(json);
                mime = "application/json";
            }

            return new Request
            {
                Method = method,
                Uri = uri,
                MediaType = mime,
                Body = content,
				Headers = headers
            };
        }

        public static IParameterizer Parameterizer { get; set; } = new ODataParameterizer();
        
        public static Request Get(string uri) => Create(HttpMethod.Get, uri);
        public static Request Get(string uri, object @params) => Get(Parameterizer.Parameterize(uri, @params));
		public static Request Get(string uri, HttpRequestHeaders headers) => Create(HttpMethod.Get, uri, headers);
		public static Request Get(string uri, object @params, HttpRequestHeaders headers) => Get(Parameterizer.Parameterize(uri, @params), headers);

		public static Request Head(string uri) => Create(HttpMethod.Head, uri);
        public static Request Head(string uri, object @params) => Head(Parameterizer.Parameterize(uri, @params));
		public static Request Head(string uri, HttpRequestHeaders headers) => Create(HttpMethod.Head, uri, headers);
		public static Request Head(string uri, object @params, HttpRequestHeaders headers) => Head(Parameterizer.Parameterize(uri, @params), headers);

		public static Request Delete(string uri) => Create(HttpMethod.Delete, uri);
        public static Request Delete(string uri, object @params) => Delete(Parameterizer.Parameterize(uri, @params));
		public static Request Delete(string uri, HttpRequestHeaders headers) => Create(HttpMethod.Delete, uri, headers);
		public static Request Delete(string uri, object @params, HttpRequestHeaders headers) => Delete(Parameterizer.Parameterize(uri, @params), headers);

		public static Request Post<T>(string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null) 
            => Create(HttpMethod.Post, uri, body, type, typeKey, serializer);
        
        public static Request Post<T>(string uri, object @params, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null) 
            => Post(Parameterizer.Parameterize(uri, @params), body, type, typeKey, serializer);

		public static Request Post<T>(string uri, T body, HttpRequestHeaders headers, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null)
	        => Create(HttpMethod.Post, uri, body, type, typeKey, serializer, headers);

		public static Request Post<T>(string uri, object @params, T body, HttpRequestHeaders headers, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null)
		  => Post(Parameterizer.Parameterize(uri, @params), body, headers, type, typeKey, serializer);

		public static Request Put<T>(string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null) 
            => Create(HttpMethod.Put, uri, body, type, typeKey, serializer);

		public static Request Put<T>(string uri, object @params, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null)
	        => Put(Parameterizer.Parameterize(uri, @params), body, type, typeKey, serializer);

		public static Request Put<T>(string uri, object @params, T body, HttpRequestHeaders headers, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null)
			=> Put(Parameterizer.Parameterize(uri, @params), body, headers, type, typeKey, serializer);

		public static Request Put<T>(string uri, T body, HttpRequestHeaders headers, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null)
			=> Create(HttpMethod.Put, uri, body, type, typeKey, serializer, headers);

		public static Request Patch<T>(string uri, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null) 
            => Create(new HttpMethod("PATCH"), uri, body, type, typeKey, serializer);
        
        public static Request Patch<T>(string uri, object @params, T body, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null) 
            => Patch(Parameterizer.Parameterize(uri, @params), body, type, typeKey, serializer);

		public static Request Patch<T>(string uri, T body, HttpRequestHeaders headers, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null)
			=> Create(new HttpMethod("PATCH"), uri, body, type, typeKey, serializer, headers);

		public static Request Patch<T>(string uri, object @params, T body, HttpRequestHeaders headers, string type = null, string typeKey = DEFAULT_TYPE_KEY, IJsonSerializer serializer = null)
			=> Patch(Parameterizer.Parameterize(uri, @params), body, headers, type, typeKey, serializer);
	}
}
