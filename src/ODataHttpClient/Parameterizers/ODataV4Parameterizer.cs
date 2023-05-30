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
        private string Literal(object value, bool suffix = true)
        {
            if (value == null)
                return "null";

            switch (value)
            {
                case bool val: return Literal(val);
                case long val: return Literal(val, suffix);
                case float val: return Literal(val, suffix);
                case double val: return Literal(val, suffix);
                case decimal val: return Literal(val, suffix);
                case byte[] val: return Literal(val);
                case string val: return Literal(val);
                case Guid val: return Literal(val);
                case TimeSpan val: return Literal(val);
                case DateTime val: return Literal(val);
                case DateTimeOffset val: return Literal(val);
                case int[] val: return LiteralArray(val);
                case long[] val: return LiteralArray(val);
                case string[] val: return LiteralArray(val);
                case Guid[] val: return LiteralArray(val);
                default: return value.ToString();
            }
        }

        private string LiteralArray<T>(T[] values) => $"({string.Join(",", values.Select(val => Literal(val, false)))})";

        private string Literal(bool value) => value ? "true" : "false";

        private string Literal(long value, bool suffix) => $"{value}{(suffix?"L":string.Empty)}";

        private string Literal(float value, bool suffix) => Invariant($"{value}{(suffix?"f":string.Empty)}");

        private string Literal(double value, bool suffix) => Invariant($"{value}{(suffix?"d":string.Empty)}");

        private string Literal(decimal value, bool suffix) => Invariant($"{value}{(suffix?"M":string.Empty)}");

        private string Literal(byte[] value) => $"binary'{String.Join("", value.Select(b => b.ToString("X2")))}'";

        private string Literal(string value) => $"'{value.Replace("@", "@@")}'";

        private string Literal(Guid value) => $"{value}";

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

            return $"duration'{duration}'";
        }

        private string Literal(DateTime value) => $"{value:yyyy-MM-ddTHH:mm:ss.fffffff}";

        private string Literal(DateTimeOffset value) => $"{value:yyyy-MM-ddTHH:mm:ss.fffffffzzz}".Replace("+","%2B");
    }
}