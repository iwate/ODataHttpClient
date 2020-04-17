using ODataHttpClient.Credentials;
using ODataHttpClient.Models;
using ODataHttpClient.Parameterizers;
using ODataHttpClient.Serializers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace ODataHttpClient
{
    public class ODataClient
    {
        private static readonly NameValueHeaderValue _responseMsgType = new NameValueHeaderValue("msgtype","response");
        private readonly HttpClient _httpClient;
        private readonly ICredentialBuilder _credentialBuilder;
        public RequestFactory RequestFactory { get; }
        public ODataClient(HttpClient httpClient) 
            : this(httpClient, JsonSerializer.Default)
        {}
        public ODataClient(HttpClient httpClient, IJsonSerializer serializer)
            : this(httpClient, null, serializer)
        {}
        public ODataClient(HttpClient httpClient, ICredentialBuilder credentialBuilder)
            : this(httpClient, credentialBuilder, JsonSerializer.Default)
        { }
        public ODataClient(HttpClient httpClient, string username, string password) 
            : this(httpClient, username, password, JsonSerializer.Default)
        {}
        public ODataClient(HttpClient httpClient, string username, string password, IJsonSerializer serializer)
            : this(httpClient, new BasicAuthCredential(username, password), serializer)
        {}
        public ODataClient(HttpClient httpClient, ICredentialBuilder credentialBuilder, IJsonSerializer serializer)
        {
            _httpClient = httpClient;
            _credentialBuilder = credentialBuilder;
            RequestFactory = new RequestFactory(serializer);
        }

        protected async Task<Response> ParseAsync(HttpStatusCode status, HttpContent content, HttpResponseHeaders headers = null)
        {
            var code = (int)status;
            var body = content != null ? await content.ReadAsByteArrayAsync() : null;
            var mime = content?.Headers.ContentType?.MediaType;

            if (code == 404)
                return Response.CreateSuccess(status, mime, (byte[])null, headers);
            
            if (code >= 400)
                return Response.CreateError(status, body, headers);
            
            return Response.CreateSuccess(status, mime, body, headers);
        }


        protected async Task<IReadOnlyList<Response>> ParseMultiAsync(MultipartMemoryStreamProvider multipart, HttpResponseHeaders headers = null)
        {
            var result = new List<Response>();
            foreach (var content in multipart.Contents)
            {
                if (content.Headers.ContentType.MediaType == "application/http")
                {
                    if (!content.Headers.ContentType.Parameters.Contains(_responseMsgType))
                        content.Headers.ContentType.Parameters.Add(_responseMsgType);
                    
                    var part = await content.ReadAsHttpResponseMessageAsync();

                    if (!part.Headers.Contains("Content-ID") 
                        && content.Headers.TryGetValues("Content-ID", out var contentId))
                    {
                        part.Headers.Add("Content-ID", contentId);
                    }

                    result.Add(await ParseAsync(part.StatusCode, part.Content, part.Headers));
                }
                else if (content.IsMimeMultipartContent())
                {
                    var children = await content.ReadAsMultipartAsync();

                    result.AddRange(await ParseMultiAsync(children, headers));
                }
            }
            return result;
        }

        public async Task<Response> SendAsync(IRequest request)
        {
            var message = request.CreateMessage();
            _credentialBuilder?.Build(_httpClient, message);

            var response = await _httpClient.SendAsync(message);
            
            return await ParseAsync(response.StatusCode, response.Content, response.Headers);
        }
        public async Task<IReadOnlyList<Response>> SendAsync(IBatchRequest batchRequest)
        {
            return await BatchAsync(batchRequest);
        }
        public async Task<IReadOnlyList<Response>> BatchAsync(IBatchRequest request)
        {
            var message = request.CreateMessage();
            _credentialBuilder?.Build(_httpClient, message);

            var response = await _httpClient.SendAsync(message);

            if (!response.Content.IsMimeMultipartContent())
                return new[] { await ParseAsync(response.StatusCode, response.Content, response.Headers) };

            var multipart = await response.Content.ReadAsMultipartAsync();

            return await ParseMultiAsync(multipart, response.Headers);
        }
        public static void UseV4Global()
        {
            JsonSerializer.Default = JsonSerializer.General;
            Request.Parameterizer = new ODataV4Parameterizer();
        }
    }
}
