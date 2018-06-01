using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ODataHttpClient.Parameterizers
{
    public class ODataParameterizer : IParameterizer
    {
        public string Parameterize(string query, object @params)
        {
            var type = @params.GetType();
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                query = Replace(query, prop.Name, prop.GetValue(@params));
            }

            return query.Replace("@@", "@");
        }

        private string Replace(string query, string name, object value)
        {
            return Regex.Replace(query, $"([^@])@{name}", "${1}" + Val(value));
        }
        private string Val(object value)
        {
            if (value == null)
                return "null";

            switch (value)
            {
                case long val: return Val(val);
                case float val: return Val(val);
                case double val: return Val(val);
                case decimal val: return Val(val);
                case byte[] val: return Val(val);
                case string val: return Val(val);
                case Guid val: return Val(val);
                case TimeSpan val: return Val(val);
                case DateTime val: return Val(val);
                case DateTimeOffset val: return Val(val);
                default: return value.ToString();
            }
        }

        private string Val(long value) => $"{value}L";

        private string Val(float value) => $"{value}f";

        private string Val(double value) => $"{value}d";

        private string Val(decimal value) => $"{value}M";

        private string Val(byte[] value) => $"binary'{String.Join("", value.Select(b => b.ToString("X2")))}'";

        private string Val(string value) => $"'{value.Replace("@", "@@")}'";

        private string Val(Guid value) => $"guid'{value}'";

        private string Val(TimeSpan value) => $"time'P{value.Days}DT{value.Hours}H{value.Minutes}M{(value.Seconds + (double)value.Milliseconds / 1000)}S'";

        private string Val(DateTime value) => $"datetime'{value:yyyy-MM-ddTHH:mm:ss.fffffff}'";

        private string Val(DateTimeOffset value) => $"datetimeoffset'{value:yyyy-MM-ddTHH:mm:ss.fffffffzzz}'";
    }
}