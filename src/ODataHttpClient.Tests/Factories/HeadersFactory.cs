using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ODataHttpClient.Tests.Factories
{
    public static class HeadersFactory
    {
        public static HttpRequestHeaders Create(string[] keys, string[] values)
        {
            var headers = new HttpRequestMessage().Headers;
            if (keys.Length != values.Length)
                return headers;

            for (int i = 0; i < keys.Length; i++)
                headers.TryAddWithoutValidation(keys[i], values[i]);

            return headers;
        }

        public static HttpRequestHeaders Create(IDictionary<string, string> keyValues)
        {
            var headers = new HttpRequestMessage().Headers;
            foreach (var pair in keyValues)
                headers.TryAddWithoutValidation(pair.Key, pair.Value);

            return headers;
        }
    }
}