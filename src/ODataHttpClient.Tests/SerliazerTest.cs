using ODataHttpClient.Models;
using ODataHttpClient.Serializers;
using System;
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
        [Fact]
        public void SerliazeByteArrayToBase64ByHistorical()
        {
            var settings = JsonSerializer.HistoricalJsonSerializerSettings;
            var json = JsonSerializer.Serialize(new byte[] { 0, 0, 0, 0, 0, 2, 74, 26 }, settings);
            Assert.Equal("\"AAAAAAACSho=\"", json);
        }
        [Fact]
        public void DeserliazeByteArrayToBase64ByHistorical()
        {
            var settings = JsonSerializer.HistoricalJsonSerializerSettings;
            var actual = JsonSerializer.Deserialize<byte[]>("\"AAAAAAACSho=\"", settings);
            var expected = new byte[] { 0, 0, 0, 0, 0, 2, 74, 26 };
            Assert.True(BitConverter.Equals(expected, actual));
        }
    }
}
