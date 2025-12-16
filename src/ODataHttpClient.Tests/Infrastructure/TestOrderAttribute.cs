using System;

namespace ODataHttpClient.Tests
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class TestOrderAttribute : Attribute
    {
        public int Order { get; }

        public TestOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
