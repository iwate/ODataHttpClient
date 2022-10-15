using System.Net.Http;

namespace ODataHttpClient.Models
{
    public  interface IBatchRequest : IRequest
    {
        bool[] AcceptNotFounds { get;}
    }
}
