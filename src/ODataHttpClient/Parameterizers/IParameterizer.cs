using System;

namespace ODataHttpClient.Parameterizers
{
    public interface IParameterizer
    {
        string Parameterize(string query, object @params);
    }
}