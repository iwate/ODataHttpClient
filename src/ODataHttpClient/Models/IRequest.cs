using System.Net.Http;
using System.Net.Http.Headers;

namespace ODataHttpClient.Models
{
    public  interface IRequest
    {
        HttpRequestHeaders Headers { get; }
        HttpRequestMessage CreateMessage();
        HttpRequestMessage CreateMessage(HttpRequestHeaders headers);
    }
}
