using System.Net.Http;

namespace ODataHttpClient.Credentials
{
    public interface ICredentialBuilder
    {
        void Build(HttpClient client, HttpRequestMessage message);
    }
}