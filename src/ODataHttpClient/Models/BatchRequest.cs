using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ODataHttpClient.Models
{
    public class BatchRequest : IRequest
    {
        public string Uri { get; }
        public ICollection<Request> Requests { get; set; } 
        public BatchRequest(string uri)
        {
            Uri = uri;
            Requests = new List<Request>();
        }

        protected HttpContent CreateContent()
        {
            var batch = new MultipartContent("mixed", "batch" + Guid.NewGuid());

            var changeset = new MultipartContent("mixed", "changeset" + Guid.NewGuid());
            var contents = Requests.Select((req, index) => {
                var message = req.CreateMessage();
                message.Headers.Add("Content-ID", $"{index + 1}");
                var content = new HttpMessageContent(message);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/http");
                content.Headers.Add("Content-Transfer-Encoding", "binary");
                return content;
            });
            foreach (var content in contents)
                changeset.Add(content);

            batch.Add(changeset);

            return batch;
        }

        public HttpRequestMessage CreateMessage()
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, Uri);

            msg.Content = CreateContent();

            return msg;
        }
    }
}
