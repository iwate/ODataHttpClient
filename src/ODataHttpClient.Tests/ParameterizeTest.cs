using System;
using ODataHttpClient.Parameterizers;
using Xunit;

namespace ODataHttpClient.Tests
{
    public class ParameterizeTest
    {
        [Fact]
        public void BoolParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=True eq @True and False eq @False", new { True = true, False = false });

            Assert.Equal("$filter=True eq true and False eq false", query);
        }

        [Fact]
        [UseCulture("de-DE")]
        public void BoolParamGeDE()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=True eq @True and False eq @False", new { True = true, False = false });

            Assert.Equal("$filter=True eq true and False eq false", query);
        }

        [Fact]
        public void LongParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value eq @Value", new { Value = 123L });

            Assert.Equal("$filter=Value eq 123L", query);
        }

        [Fact]
        public void FloatParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value eq @Value", new { Value = 1.23f });

            Assert.Equal("$filter=Value eq 1.23f", query);
        }

        [Fact]
        public void DoubleParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value eq @Value", new { Value = 1.23d });

            Assert.Equal("$filter=Value eq 1.23d", query);
        }

        [Fact]
        public void DecimalParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value eq @Value", new { Value = 1.23m });

            Assert.Equal("$filter=Value eq 1.23M", query);
        }
        
        [Fact]
        public void BinaryParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value eq @Value", new { Value = new byte[] { 0x0a, 0x01, 0xff } });

            Assert.Equal("$filter=Value eq binary'0A01FF'", query);
        }

        [Fact]
        public void StringParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value eq @Value", new { Value = "Hello, World!"});

            Assert.Equal("$filter=Value eq 'Hello, World!'", query);
        }

        [Fact]
        public void GuidParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value eq @Value", new { Value = Guid.Empty });

            Assert.Equal("$filter=Value eq guid'00000000-0000-0000-0000-000000000000'", query);
        }

        [Fact]
        public void TimeSpanParam()
        {
            var odata = new ODataParameterizer();

            Assert.Equal("$filter=Value eq time'P30D'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromDays(30) }));
            Assert.Equal("$filter=Value eq time'PT1H'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromHours(1) }));
            Assert.Equal("$filter=Value eq time'PT15M'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromMinutes(15) }));
            Assert.Equal("$filter=Value eq time'PT1S'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromSeconds(1) }));
            Assert.Equal("$filter=Value eq time'PT0.5S'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromMilliseconds(500) }));
            Assert.Equal("$filter=Value eq time'PT1.5S'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromMilliseconds(1500) }));
            Assert.Equal("$filter=Value eq time'PT0S'", odata.Parameterize("$filter=Value eq @Value", new { Value = default(TimeSpan) }));
        }

        [Fact]
        [UseCulture("it-IT")]
        public void TimeSpanParamItIT()
        {
            var odata = new ODataParameterizer();

            Assert.Equal("$filter=Value eq time'P30D'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromDays(30) }));
            Assert.Equal("$filter=Value eq time'PT1H'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromHours(1) }));
            Assert.Equal("$filter=Value eq time'PT15M'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromMinutes(15) }));
            Assert.Equal("$filter=Value eq time'PT1S'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromSeconds(1) }));
            Assert.Equal("$filter=Value eq time'PT0.5S'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromMilliseconds(500) }));
            Assert.Equal("$filter=Value eq time'PT1.5S'", odata.Parameterize("$filter=Value eq @Value", new { Value = TimeSpan.FromMilliseconds(1500) }));
            Assert.Equal("$filter=Value eq time'PT0S'", odata.Parameterize("$filter=Value eq @Value", new { Value = default(TimeSpan) }));
        }

        [Fact]
        public void DateTimeParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value eq @Value", new { Value = DateTime.MaxValue });

            Assert.Equal("$filter=Value eq datetime'9999-12-31T23:59:59.9999999'", query);
        }

        [Fact]
        public void DateTimeOffsetParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value eq @Value", new { Value = DateTimeOffset.MaxValue });

            Assert.Equal("$filter=Value eq datetimeoffset'9999-12-31T23:59:59.9999999+00:00'", query);
        }

        [Fact]
        public void All()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value1 eq @Value and Value2 eq @Value and Value3 eq @Value", new { Value = 123 });

            Assert.Equal("$filter=Value1 eq 123 and Value2 eq 123 and Value3 eq 123", query);
        }

        [Fact]
        public void Complex()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value1 eq @Value1 and Value2 eq @Value2 and Value3 eq @Value3", new { Value1 = 123L, Value2 = 1.23d, Value3 = 1.23m });

            Assert.Equal("$filter=Value1 eq 123L and Value2 eq 1.23d and Value3 eq 1.23M", query);
        }

        [Fact]
        public void NullableParam()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value1 eq @Value1 and Value2 eq @Value2", new { Value1 = (int?)123, Value2 = (int?)null });

            Assert.Equal("$filter=Value1 eq 123 and Value2 eq null", query);
        }

        [Fact]
        public void Email()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value1 eq @Value1 and Value2 eq @Value2", new { Value1 = "email@Value2.com", Value2 = 123 });

            Assert.Equal("$filter=Value1 eq 'email@Value2.com' and Value2 eq 123", query);
        }

        [Fact]
        public void Dollar()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value1 eq @Value1", new { Value1 = "$value$" });

            Assert.Equal("$filter=Value1 eq '$value$'", query);
        }

        [Fact]
        public void Dollars()
        {
            var odata = new ODataParameterizer();
            var query = odata.Parameterize("$filter=Value1 eq @Value1", new { Value1 = "$$value$$" });

            Assert.Equal("$filter=Value1 eq '$$value$$'", query);
        }
    }
}