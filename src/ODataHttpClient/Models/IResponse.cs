using System.Net.Http.Headers;

namespace ODataHttpClient.Models
{
    public interface IResponse
    {
        HttpRequestHeaders Headers { get; }
    }
}