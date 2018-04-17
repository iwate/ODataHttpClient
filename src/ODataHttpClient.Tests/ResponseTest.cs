using System;
using System.IO;
using System.Linq;
using System.Net;
using ODataHttpClient.Models;
using ODataHttpClient.Serializers;
using Xunit;

namespace ODataHttpClient.Tests
{
    public class ResponseTest
    {
        [Fact]
        public void ReadJsonAsDynamic()
        {
            var response = Response.CreateSuccess(HttpStatusCode.OK, "application/json", JsonSerializer.Default.Serialize(new { Text = "Hello, World!" }));
            var obj =  response.ReadAs<dynamic>();
            string text = obj.Text;

            Assert.Equal("Hello, World!", text);
        }
        [Fact]
        public void ReadJsonAsWithPath1()
        {
            var response = Response.CreateSuccess(HttpStatusCode.OK, "application/json", JsonSerializer.Default.Serialize(new { Text = "Hello, World!" }));
            var text =  response.ReadAs<string>("$.Text");

            Assert.Equal("Hello, World!", text);
        }
        [Fact]
        public void ReadJsonAsWithPath2()
        {
            var response = Response.CreateSuccess(HttpStatusCode.OK, "application/json", "{\"odata.type\": \"ODataDemo.Product\"}");
            var type =  response.ReadAs<string>("$['odata.type']");

            Assert.Equal("ODataDemo.Product", type);
        }
        [Fact]
        public void ReadTextAsString()
        {
            var response = Response.CreateSuccess(HttpStatusCode.OK, "text/plain", "100");

            Assert.Equal("100", response.ReadAs<string>());
        }
        [Fact]
        public void ReadTextAsNullString()
        {
            var response = Response.CreateSuccess(HttpStatusCode.NotFound, "text/plain", (string)null);

            Assert.Null(response.ReadAs<string>());
        }
        [Fact]
        public void ReadTextAsInt()
        {
            var response = Response.CreateSuccess(HttpStatusCode.OK, "text/plain", "100");

            Assert.Equal(100, response.ReadAs<int>());
            Assert.Equal(100, response.ReadAs<int?>());
        }
        [Fact]
        public void ReadTextAsLong()
        {
            var response = Response.CreateSuccess(HttpStatusCode.OK, "text/plain", "100");
            
            Assert.Equal(100L, response.ReadAs<long>());
            Assert.Equal(100L, response.ReadAs<long?>());
        }
        [Fact]
        public void ReadTextAsDouble()
        {
            var response = Response.CreateSuccess(HttpStatusCode.OK, "text/plain", "100.0");
            
            Assert.Equal(100d, response.ReadAs<double>());
            Assert.Equal(100d, response.ReadAs<double?>());
        }
        [Fact]
        public void ReadTextAsDecimal()
        {
            var response = Response.CreateSuccess(HttpStatusCode.OK, "text/plain", "100.0");
            
            Assert.Equal(100m, response.ReadAs<decimal>());
            Assert.Equal(100m, response.ReadAs<decimal?>());
        }
        [Fact]
        public void ReadTextAsDateTime()
        {
            var response = Response.CreateSuccess(HttpStatusCode.OK, "text/plain", "2018-01-01T00:00:00.000");
            
            Assert.Equal(new DateTime(2018,1,1,0,0,0,0), response.ReadAs<DateTime>());
            Assert.Equal(new DateTime(2018,1,1,0,0,0,0), response.ReadAs<DateTime>());
        }
        [Fact]
        public void ReadTextAsDateTimeOffset()
        {
            var jst = Response.CreateSuccess(HttpStatusCode.OK, "text/plain", "2018-01-01T00:00:00.000+09:00");
            var utc = Response.CreateSuccess(HttpStatusCode.OK, "text/plain", "2018-01-01T00:00:00.000Z");
            
            Assert.Equal(new DateTimeOffset(2018,1,1,0,0,0,0, TimeSpan.FromHours(9)), jst.ReadAs<DateTimeOffset>());
            Assert.Equal(new DateTimeOffset(2018,1,1,0,0,0,0, TimeSpan.FromHours(9)), jst.ReadAs<DateTimeOffset?>());
            Assert.Equal(new DateTimeOffset(2018,1,1,0,0,0,0, TimeSpan.FromHours(0)), utc.ReadAs<DateTimeOffset>());
            Assert.Equal(new DateTimeOffset(2018,1,1,0,0,0,0, TimeSpan.FromHours(0)), utc.ReadAs<DateTimeOffset?>());
        }
        [Fact]
        public void ReadBinaryAsByteArray()
        {
            var binary = new byte[] { 0, 1, 2, 3, 4 };
            var response = Response.CreateSuccess(HttpStatusCode.OK, "application/octet-stream", binary);
            Assert.True(Enumerable.SequenceEqual(binary, response.ReadAs<byte[]>()));
        }
        [Fact]
        public void ReadBinaryAsStream()
        {
            var binary = new byte[] { 0, 1, 2, 3, 4 };
            var response = Response.CreateSuccess(HttpStatusCode.OK, "application/octet-stream", binary);
            using (var stream = response.ReadAs<Stream>())
            {
                Assert.Equal(0, stream.ReadByte());
                Assert.Equal(1, stream.ReadByte());
                Assert.Equal(2, stream.ReadByte());
                Assert.Equal(3, stream.ReadByte());
                Assert.Equal(4, stream.ReadByte());
            }
               
        }
    }
}