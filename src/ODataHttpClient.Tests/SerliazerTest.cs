using ODataHttpClient.Models;
using ODataHttpClient.Serializers;
using System.Net;
using System.Net.Http;
using Xunit;

namespace ODataHttpClient.Tests
{
    public class SerliazerTest
    {
        [Fact]
        public void SerializeAsHistorical()
        {
            var odata = new ODataClient(new HttpClient(), JsonSerializer.HistoricalJsonSerializerSettings);
            Assert.Equal("\"100\"", odata.RequestFactory.Post("", 100L).Body);
            Assert.Equal("\"100.5\"", odata.RequestFactory.Post("", 100.5d).Body);
            Assert.Equal("\"100.5\"", odata.RequestFactory.Post("", 100.5m).Body);
            
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "\"100\"").ReadAs<long>()));
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "\"100.5\"").ReadAs<double>()));
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "\"100.5\"").ReadAs<decimal>()));
        }
        [Fact]
        public void SerializeAsGeneral()
        {
            var odata = new ODataClient(new HttpClient(), JsonSerializer.GeneralJsonSerializerSettings);
            Assert.Equal("100", odata.RequestFactory.Post("", 100L).Body);
            Assert.Equal("100.5", odata.RequestFactory.Post("", 100.5d).Body);
            Assert.Equal("100.5", odata.RequestFactory.Post("", 100.5m).Body);

            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "100").ReadAs<long>()));
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "100.5").ReadAs<double>()));
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "100.5").ReadAs<decimal>()));
        }
    }
}
