using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ODataHttpClient.Models;
using Xunit;

namespace ODataHttpClient.Tests
{
    public class RequestTest
    {
        const string uri = "http://services.odata.org/V3/OData/OData.svc/Products";
        const string batchUri = "http://services.odata.org/V3/OData/OData.svc/$batch";
		const string headerKey = "If-Match";
		const string headerValue = "*";
        static IReadOnlyDictionary<string, string>  headers = new Dictionary<string, string> { [headerKey] = headerValue };

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
        public async Task ContentTypePlainText()
        {
            var message = Request.Create(HttpMethod.Post, uri, "Hello, World!").CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("Hello, World!", await message.Content.ReadAsStringAsync());

            message = Request.Create(HttpMethod.Post, uri, 100).CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("100", await message.Content.ReadAsStringAsync());

            message = Request.Create(HttpMethod.Post, uri, 1.23f).CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("1.23", await message.Content.ReadAsStringAsync());

            message = Request.Create(HttpMethod.Post, uri, 1.23d).CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("1.23", await message.Content.ReadAsStringAsync());

            message = Request.Create(HttpMethod.Post, uri, 1.23m).CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("1.23", await message.Content.ReadAsStringAsync());
        }
        [Fact]
        [UseCulture("it-IT")]
        public async Task ContentTypePlainTextItIT()
        {
            var message = Request.Create(HttpMethod.Post, uri, "Hello, World!").CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("Hello, World!", await message.Content.ReadAsStringAsync());

            message = Request.Create(HttpMethod.Post, uri, 100).CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("100", await message.Content.ReadAsStringAsync());

            message = Request.Create(HttpMethod.Post, uri, 1.23f).CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("1.23", await message.Content.ReadAsStringAsync());

            message = Request.Create(HttpMethod.Post, uri, 1.23d).CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("1.23", await message.Content.ReadAsStringAsync());

            message = Request.Create(HttpMethod.Post, uri, 1.23m).CreateMessage();
            Assert.Equal("text/plain", message.Content.Headers.ContentType.MediaType);
            Assert.Equal("1.23", await message.Content.ReadAsStringAsync());
        }
        [Fact]
        public void CreateGetRequest()
        {
            var request = Request.Get(uri);
            Assert.Equal(HttpMethod.Get, request.Method);
        }
		[Fact]
		public void CreateGetRequestWithHeader()
		{
			var request = Request.Get(uri, headers);
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.True(((Request)request).Headers.ContainsKey(headerKey));
		}
		[Fact]
        public void CreateHeadRequest()
        {
            var request = Request.Head(uri);
            Assert.Equal(HttpMethod.Head, request.Method);
        }
		[Fact]
		public void CreateHeadRequestWithHeader()
		{
			var request = Request.Head(uri, headers);
			Assert.Equal(HttpMethod.Head, request.Method);
			Assert.True(((Request)request).Headers.ContainsKey(headerKey));
		}
		[Fact]
        public void CreateDeleteRequest()
        {
            var request = Request.Delete(uri);
            Assert.Equal(HttpMethod.Delete, request.Method);
        }
		[Fact]
		public void CreateDeleteRequestWithHeader()
		{
			var request = Request.Delete(uri, headers);
			Assert.Equal(HttpMethod.Delete, request.Method);
			Assert.True(((Request)request).Headers.ContainsKey(headerKey));
		}
		[Fact]
        public void CreatePostRequest()
        {
            var request = Request.Post(uri, new {});
            Assert.Equal(HttpMethod.Post, request.Method);
        }
		[Fact]
		public void CreatePostRequestWithHeader()
		{
			var request = Request.Post(uri, new { }, headers);
			Assert.Equal(HttpMethod.Post, request.Method);
			Assert.True(((Request)request).Headers.ContainsKey(headerKey));
		}
		[Fact]
        public void CreatePutRequest()
        {
            var request = Request.Put(uri, new {});
            Assert.Equal(HttpMethod.Put, request.Method);
        }
		[Fact]
		public void CreatePutRequestWithHeader()
		{
			var request = Request.Put(uri, new { }, headers);
			Assert.Equal(HttpMethod.Put, request.Method);
			Assert.True(((Request)request).Headers.ContainsKey(headerKey));
		}
		[Fact]
        public void CreatePatchRequest()
        {
            var request = Request.Patch(uri, new {});
            Assert.Equal(new HttpMethod("PATCH"), request.Method);
        }
		[Fact]
		public void CreatePatchRequestWithHeader()
		{
			var request = Request.Patch(uri, new { }, headers);
			Assert.Equal(new HttpMethod("PATCH"), request.Method);
			Assert.True(((Request)request).Headers.ContainsKey(headerKey));
		}
		[Fact]
        public async Task BatchRequest()
        {
            var message = new BatchRequest(batchUri)
            {
                Requests =
                {
                    Request.Post(uri, new {}),
                    Request.Post(uri, new {})
                },
                Headers = new Dictionary<string, string> {
                    ["Isolation"] = "snapshot",
                    ["Prefer"] = "odata.continue-on-error",
                }
            }.CreateMessage();

            Assert.True(message.Content.IsMimeMultipartContent());

            Assert.Equal("snapshot", message.Headers.GetValues("Isolation").First());
            Assert.Equal("odata.continue-on-error", message.Headers.GetValues("Prefer").First());

            var multipart1 = await message.Content.ReadAsMultipartAsync();

            Assert.Single(multipart1.Contents);
            Assert.True(multipart1.Contents.First().IsMimeMultipartContent());
            
            var multipart2 = await multipart1.Contents.First().ReadAsMultipartAsync();
            
            Assert.Equal(2, multipart2.Contents.Count);

            multipart2.Contents[0].Headers.ContentType.Parameters.Add(new NameValueHeaderValue("msgtype","request"));
            multipart2.Contents[1].Headers.ContentType.Parameters.Add(new NameValueHeaderValue("msgtype","request"));
            Assert.True(multipart2.Contents[0].IsHttpRequestMessageContent());
            Assert.True(multipart2.Contents[1].IsHttpRequestMessageContent());

            var req1 = await multipart2.Contents[0].ReadAsHttpRequestMessageAsync();
            var req2 = await multipart2.Contents[1].ReadAsHttpRequestMessageAsync();

            Assert.Equal("1", req1.Headers.GetValues("Content-ID")?.FirstOrDefault());
            Assert.Equal("2", req2.Headers.GetValues("Content-ID")?.FirstOrDefault());
        }
        [Fact]
		public void CreateRequestWithHeaderByFactory()
		{
			var body = "text";
			var request = new RequestFactory()
				.Create(HttpMethod.Get, uri,
					body, headers:headers);
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.True(((Request)request).Headers.ContainsKey(headerKey));
		}
	}
}