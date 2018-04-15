using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ODataHttpClient.Credentials
{
    public class BasicAuthCredential : ICredentialBuilder
    {
        private readonly AuthenticationHeaderValue _authroization;
        public BasicAuthCredential(string username, string password)
        {
            _authroization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));
        }

        public void Build(HttpClient client, HttpRequestMessage message)
        {
            message.Headers.Authorization = _authroization;
        }
    }
}