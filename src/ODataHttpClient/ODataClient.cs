using ODataHttpClient.Credentials;
using ODataHttpClient.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ODataHttpClient
{
    public class ODataClient
    {
        private static readonly NameValueHeaderValue _responseMsgType = new NameValueHeaderValue("msgtype","response");
        private readonly HttpClient _httpClient;
        private readonly ICredentialBuilder _credentialBuilder;
        public ODataClient(HttpClient httpClient) 
            : this(httpClient, null)
        {}
        public ODataClient(HttpClient httpClient, string username, string password) 
            : this(httpClient, new BasicAuthCredential(username, password))
        {}
        public ODataClient(HttpClient httpClient, ICredentialBuilder credentialBuilder)
        {
            _httpClient = httpClient;
            _credentialBuilder = credentialBuilder;
        }

        protected async Task<Response> ParseAsync(HttpStatusCode status, HttpContent content)
        {
            var code = (int)status;
            var body = content != null ? await content.ReadAsStringAsync() : null;
            var mime = content?.Headers.ContentType?.MediaType;
            
            if (code >= 400 && code != 404)
                return Response.CreateError(status, body);
            
            return Response.CreateSuccess(status, mime, body);
        }
        protected async Task<IEnumerable<Response>> ParseMultiAsync(MultipartMemoryStreamProvider multipart)
        {
            var result = new List<Response>();
            foreach (var content in multipart.Contents)
            {
                if (content.Headers.ContentType.MediaType == "application/http")
                {
                    if (!content.Headers.ContentType.Parameters.Contains(_responseMsgType))
                        content.Headers.ContentType.Parameters.Add(_responseMsgType);
                    
                    var part = await content.ReadAsHttpResponseMessageAsync();

                    result.Add(await ParseAsync(part.StatusCode, part.Content));
                }
                else if (content.IsMimeMultipartContent())
                {
                    var children = await content.ReadAsMultipartAsync();

                    result.AddRange(await ParseMultiAsync(children));
                }
            }
            return result;
        }

        public async Task<Response> SendAsync(IRequest request)
        {
            var message = request.CreateMessage();

            _credentialBuilder?.Build(_httpClient, message);

            var response = await _httpClient.SendAsync(message);
            
            return await ParseAsync(response.StatusCode, response.Content);
        }
        public async Task<IEnumerable<Response>> BatchAsync(IRequest request)

        {
            var message = request.CreateMessage();
            
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _credentialBuilder?.Build(_httpClient, message);

            var response = await _httpClient.SendAsync(message);

            if (!response.Content.IsMimeMultipartContent())
                return new[] { await ParseAsync(response.StatusCode, response.Content) };

            var multipart = await response.Content.ReadAsMultipartAsync();

            return await ParseMultiAsync(multipart);
        }
    }
}
