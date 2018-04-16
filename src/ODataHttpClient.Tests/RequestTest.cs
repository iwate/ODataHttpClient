using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using ODataHttpClient.Models;
using Xunit;

namespace ODataHttpClient.Tests
{
    public class RequestTest
    {
        const string uri = "http://services.odata.org/V3/OData/OData.svc/Products";
        const string batchUri = "http://services.odata.org/V3/OData/OData.svc/$batch";
        [Fact]
        public void NonBodyRequest()
        {
            var request = Request.Create(HttpMethod.Get, uri);
            Assert.Null(request.Body);
        }
        [Fact]
        public void AcceptJson()
        {
            var request = Request.Create(HttpMethod.Get, uri);
            var message = request.CreateMessage();
            Assert.NotNull(message.Headers.Accept);
            Assert.Contains(new MediaTypeWithQualityHeaderValue("application/json"), message.Headers.Accept);
        }
        [Fact]
        public void AcceptText()
        {
            var request = Request.Create(HttpMethod.Get, uri);
            var message = request.CreateMessage();
            Assert.NotNull(message.Headers.Accept);
            Assert.Contains(new MediaTypeWithQualityHeaderValue("text/plain"), message.Headers.Accept);
        }
        [Fact]
        public void ContentTypeJson()
        {
            var request = Request.Create(HttpMethod.Post, uri, new {});
            var message = request.CreateMessage();
            Assert.True(message.Content.Headers.ContentType.MediaType == "application/json");
        }
        [Fact]
        public void CreateGetRequest()
        {
            var request = Request.Get(uri);
            Assert.Equal(HttpMethod.Get, request.Method);
        }
        [Fact]
        public void CreateHeadRequest()
        {
            var request = Request.Head(uri);
            Assert.Equal(HttpMethod.Head, request.Method);
        }
        [Fact]
        public void CreateDeleteRequest()
        {
            var request = Request.Delete(uri);
            Assert.Equal(HttpMethod.Delete, request.Method);
        }
        [Fact]
        public void CreatePostRequest()
        {
            var request = Request.Post(uri, new {});
            Assert.Equal(HttpMethod.Post, request.Method);
        }
        [Fact]
        public void CreatePutRequest()
        {
            var request = Request.Put(uri, new {});
            Assert.Equal(HttpMethod.Put, request.Method);
        }
        [Fact]
        public void CreatePatchRequest()
        {
            var request = Request.Patch(uri, new {});
            Assert.Equal(new HttpMethod("PATCH"), request.Method);
        }
        [Fact]
        public void BatchRequest()
        {
            var message = new BatchRequest(batchUri)
            {
                Requests =
                {
                    Request.Post(uri, new {}),
                    Request.Post(uri, new {})
                }
            }.CreateMessage();

            Assert.True(message.Content.IsMimeMultipartContent());

            var multipart1 = message.Content.ReadAsMultipartAsync().Result;

            Assert.Single(multipart1.Contents);
            Assert.True(multipart1.Contents.First().IsMimeMultipartContent());
            
            var multipart2 = multipart1.Contents.First().ReadAsMultipartAsync().Result;
            
            Assert.Equal(2, multipart2.Contents.Count);

            multipart2.Contents[0].Headers.ContentType.Parameters.Add(new NameValueHeaderValue("msgtype","request"));
            multipart2.Contents[1].Headers.ContentType.Parameters.Add(new NameValueHeaderValue("msgtype","request"));
            Assert.True(multipart2.Contents[0].IsHttpRequestMessageContent());
            Assert.True(multipart2.Contents[1].IsHttpRequestMessageContent());

            var req1 = multipart2.Contents[0].ReadAsHttpRequestMessageAsync().Result;
            var req2 = multipart2.Contents[1].ReadAsHttpRequestMessageAsync().Result;

            Assert.Equal("1", req1.Headers.GetValues("Content-ID")?.FirstOrDefault());
            Assert.Equal("2", req2.Headers.GetValues("Content-ID")?.FirstOrDefault());
        }
    }
}