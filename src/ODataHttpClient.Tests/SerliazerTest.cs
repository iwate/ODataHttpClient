using ODataHttpClient.Models;
using ODataHttpClient.Serializers;
using System.Linq;
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
            var odata = new ODataClient(new HttpClient(), JsonSerializer.Historical);
            Assert.Equal("{\"a\":\"100\"}", odata.RequestFactory.Post("", new { a = 100L }).Body);
            Assert.Equal("{\"a\":\"100.5\"}", odata.RequestFactory.Post("", new { a = 100.5d }).Body);
            Assert.Equal("{\"a\":\"100.5\"}", odata.RequestFactory.Post("", new { a = 100.5m }).Body);
            
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "\"100\"").ReadAs<long>()));
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "\"100.5\"").ReadAs<double>()));
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "\"100.5\"").ReadAs<decimal>()));
        }
        [Fact]
        public void SerializeAsGeneral()
        {
            var odata = new ODataClient(new HttpClient(), JsonSerializer.General);
            Assert.Equal("100", odata.RequestFactory.Post("", 100L).Body);
            Assert.Equal("100.5", odata.RequestFactory.Post("", 100.5d).Body);
            Assert.Equal("100.5", odata.RequestFactory.Post("", 100.5m).Body);

            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "100").ReadAs<long>()));
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "100.5").ReadAs<double>()));
            Assert.Null(Record.Exception(() => Response.CreateSuccess(HttpStatusCode.OK, "application/json", "100.5").ReadAs<decimal>()));
        }
        [Fact]
        public void SerliazeByteArrayToBase64ByHistorical()
        {
            var json = JsonSerializer.Historical.Serialize(new byte[] { 0, 0, 0, 0, 0, 2, 74, 26 });
            Assert.Equal("\"AAAAAAACSho=\"", json);
        }
        [Fact]
        public void DeserliazeByteArrayToBase64ByHistorical()
        {
            var actual = JsonSerializer.Historical.Deserialize<byte[]>("\"AAAAAAACSho=\"");
            var expected = new byte[] { 0, 0, 0, 0, 0, 2, 74, 26 };
            Assert.True(Enumerable.SequenceEqual(expected, actual));
        }
        [Fact]
        public void SerliazeByteArrayToBase64ByGeneral()
        {
            var json = JsonSerializer.General.Serialize(new byte[] { 0, 0, 0, 0, 0, 2, 74, 26 });
            Assert.Equal("\"AAAAAAACSho=\"", json);
        }
        [Fact]
        public void DeserliazeByteArrayToBase64ByGeneral()
        {
            var actual = JsonSerializer.General.Deserialize<byte[]>("\"AAAAAAACSho=\"");
            var expected = new byte[] { 0, 0, 0, 0, 0, 2, 74, 26 };
            Assert.True(Enumerable.SequenceEqual(expected, actual));
        }
    }
}
