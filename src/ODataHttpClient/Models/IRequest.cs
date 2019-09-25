using System.Net.Http;

namespace ODataHttpClient.Models
{
    public  interface IRequest
    {
        HttpRequestMessage CreateMessage();
    }
}
