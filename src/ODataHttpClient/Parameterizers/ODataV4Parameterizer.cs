using System;
using System.Linq;
using System.Text.RegularExpressions;
using static System.FormattableString;

namespace ODataHttpClient.Parameterizers
{
    public class ODataV4Parameterizer : IParameterizer
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
            return Regex.Replace(query, $"([^@])@{name}", "${1}" + Literal(value));
        }
        private string Literal(object value)
        {
            if (value == null)
                return "null";

            switch (value)
            {
                case bool val: return Literal(val);
                case long val: return Literal(val);
                case float val: return Literal(val);
                case double val: return Literal(val);
                case decimal val: return Literal(val);
                case byte[] val: return Literal(val);
                case string val: return Literal(val);
                case Guid val: return Literal(val);
                case TimeSpan val: return Literal(val);
                case DateTime val: return Literal(val);
                case DateTimeOffset val: return Literal(val);
                default: return value.ToString();
            }
        }

        private string Literal(bool value) => value ? "true" : "false";

        private string Literal(long value) => $"{value}L";

        private string Literal(float value) => Invariant($"{value}f");

        private string Literal(double value) => Invariant($"{value}d");

        private string Literal(decimal value) => Invariant($"{value}M");

        private string Literal(byte[] value) => $"binary'{String.Join("", value.Select(b => b.ToString("X2")))}'";

        private string Literal(string value) => $"'{value.Replace("@", "@@")}'";

        private string Literal(Guid value) => $"guid'{value}'";

        private string Literal(TimeSpan value)
        {
            var duration = string.Join("", 
                new[]{ 
                    "P", 
                    $"{value.Days}D", 
                    "T", 
                    $"{value.Hours}H", 
                    $"{value.Minutes}M", 
                    Invariant($"{(value.Seconds + (double)value.Milliseconds / 1000)}S")
                }.Where(v => v.Length != 2 || v[0] != '0'));
                
	        duration = duration == "PT" ? "PT0S" : duration[duration.Length-1] == 'T' ? duration.Substring(0,duration.Length-1) : duration;

            return $"time'{duration}'";
        }

        private string Literal(DateTime value) => $"{value:yyyy-MM-ddTHH:mm:ss.fffffff}";

        private string Literal(DateTimeOffset value) => $"{value:yyyy-MM-ddTHH:mm:ss.fffffffzzz}".Replace("+","%2B");
    }
}