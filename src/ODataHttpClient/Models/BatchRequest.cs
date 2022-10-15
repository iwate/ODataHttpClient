using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ODataHttpClient.Models
{
    public class BatchRequest : IBatchRequest
    {
        public string Uri { get; }
        public ICollection<Request> Requests { get; set; }
        public IReadOnlyDictionary<string, string> Headers { get; private set; }
        public bool AcceptNotFound => false;
        public bool[] AcceptNotFounds => Requests.Select((req) => req.AcceptNotFound).ToArray();
        public BatchRequest(string uri)
        {
            Uri = uri;
            Requests = new List<Request>();
        }

        protected HttpContent CreateContent()
        {
            HttpMessageContent create(IRequest req, int index)
            {
                var message = req.CreateMessage();
                message.Headers.Add("Content-ID", $"{index + 1}");
                var content = new HttpMessageContent(message);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/http");
                content.Headers.Add("Content-Transfer-Encoding", "binary");
                return content;
            }

            MultipartContent batch = new MultipartContent("mixed", "batch" + Guid.NewGuid());
            MultipartContent changeset = null;
            foreach ((var req, int index) in Requests.Select((req, index) => (req, index)))
            {
                if (req.Method == HttpMethod.Get || req.Method == HttpMethod.Head)
                {
                    if (changeset != null)
                    {
                        batch.Add(changeset);
                        changeset = null;
                    }
                    batch.Add(create(req, index));
                }
                else
                {
                    if (changeset == null)
                    {
                        changeset = new MultipartContent("mixed", "changeset" + Guid.NewGuid());
                    }
                    changeset.Add(create(req, index));
                }
            }

            if (changeset != null)
            {
                batch.Add(changeset);
                changeset = null;
            }

            return batch;
        }

        public HttpRequestMessage CreateMessage()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, Uri)
            {
                Content = CreateContent()
            };

            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/mixed"));
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (Headers != null)
            {
                foreach (var pair in Headers)
                    message.Headers.Add(pair.Key, pair.Value);
            }
            
            return message;
        }
    }
}
