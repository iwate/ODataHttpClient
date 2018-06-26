[![CircleCI](https://circleci.com/gh/iwate/ODataHttpClient/tree/master.svg?style=svg)](https://circleci.com/gh/iwate/ODataHttpClient/tree/master)
[![NuGet version](https://badge.fury.io/nu/ODataHttpClient.svg)](https://badge.fury.io/nu/ODataHttpClient)

The simplest implementation of OData client.

## Install

    $ dotnet add package ODataHttpClient
    PS> Install-Package  ODataHttpClient

## Restrictions

- Not support XML (JSON only)
- Not support Query Builder

## Usage

### Simple Request

    var client = new HttpClient();
    var odata = new ODataClient(client);
    var reqest = Request.Get($"{endpoint}/Products?$inlinecount=allpages");
    var response = await odata.SendAsync(request);

    if (response.Success) 
    {
        var total = response.ReadAs<int>("$['odata.count']");
        var products = response.ReadAs<IEnumerable<Product>>("$.value");
    }

### Batch Request

    var client = new HttpClient();
    var odata = new ODataClient(client);
    var batch = new BatchRequest($"{endpoint}/$batch")
    {
        Requests = new []
        {
            Request.Post($"{endpoint}/Products", new 
            { 
                Name = "Sample"
            }),
            Request.Post($"{endpoint}/$1/$links/Categories", new 
            {
                url = $"{endpoint}/Categories(0)"
            })
        }
    };

    var responses = await odata.SendAsync(batch);
    // You can also use BatchAsync. 
    // var responses = await odata.BatchAsync(batch);

    if (responses.All(res => res.Success)) 
    {
        var id = response.First().ReadAs<int>("$.Id");
    }
### Parameterized Uri

    var client = new HttpClient();
    var odata = new ODataClient(client);
    var reqest = Request.Get($"{endpoint}/Products?$filter=ReleaseDate ge @Date", new { Date = new DateTime(2000, 1, 1) });
    var response = await odata.SendAsync(request);

    if (response.Success) 
    {
        ...
    }

And you can use `@@` as escape for `@`.

### Set OData Element Type
In default, OData element type is not contains in payload. If you want add element type, you sould use `type` parameter of Request factory method.

    Request.Post("...", payload, type: "ODataDemo.Product")

### Change OData Element Type Key
In default, OData element type key is `odata.type`. If you want change key to other, you should use `typeKey` parameter of Request factory method.

    Request.Post("...", payload, type: "ODataDemo.Product", typeKey: "@odata.type")

## Credential

### Basic Auth

    var odata = new ODataClient(client, "username", "password");

### Custom
You should modify default settings of client which be passed to ODataClient constructor or implemet `ICredentialBuilder`

    public class CustomCredentialBuilder : ODataHttpClient.Credentials.ICredentialBuilder
    {
        public void Build(HttpClient client, HttpRequestMessage message)
        {
            // implement custom credential logic.
        }
    }

And pass the builder instance to 2nd parameter constructor.

    var odata = new ODataClient(client, new CustomCredentialBuilder());
    
## Json Serializer Settings
In default, ODataHttpClient use [OData V2 JSON Serialization Format](http://www.odata.org/documentation/odata-version-2-0/json-format/#PrimitiveTypes).  
If you change general json format, can select a way of three.

### 1. Global level settings

    ODataHttpClient.Serializers.JsonSerializer.Default = ODataHttpClient.Serializers.JsonSerializer.General;

### 2. Instance level settings

    var odata = new ODataHttpClient(httpClient, ODataHttpClient.Serializers.JsonSerializer.General);
    var request = odata.RequestFacotry.Get("..."); // MUST use RequestFactory

### 3. Request level settings

    var serializer = ODataHttpClient.Serializers.JsonSerializer.General;
    var request = Request.Get("...", serializer); // pass serializer
    var response = await odata.SendAsync(request);
    var data = response.ReadAs<dynamic>(serializer); // pass serializer
