namespace ODataHttpClient.Serializers
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);

        T Deserialize<T>(string json);
        T Deserialize<T>(string json, string path);
    }
}
