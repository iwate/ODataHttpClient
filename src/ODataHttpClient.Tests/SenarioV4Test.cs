using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ODataHttpClient.Models;
using Xunit;

namespace ODataHttpClient.Tests
{
    public class SenarioV4Test
    {
        const string endpoint = "https://services.odata.org/V4/OData/(S(sdwxdzr12jf3tofcuinigded))/OData.svc/";
        static HttpClient httpClient = new HttpClient();
        static ODataClient odata = new ODataClient(httpClient);
        public SenarioV4Test()
        {
            ODataClient.UseV4Global();
        }
        [Fact]
        public async Task GetProducts()
        {
            var response = await odata.SendAsync(Request.Get($"{endpoint}/Products"));

            Assert.True(response.Success);

            var products = response.ReadAs<IEnumerable<dynamic>>("$.value");

            Assert.NotNull(products);
            Assert.True(products.Count() > 0);
        }
        public class Product { public int Id { get; set; } }
        [Fact]
        public async Task GetProductsWithModel()
        {
            var response = await odata.SendAsync(Request.Get($"{endpoint}/Products"));

            Assert.True(response.Success);

            var products = response.ReadAs<IEnumerable<Product>>("$.value");

            Assert.NotNull(products);
            Assert.True(products.Count() > 0);
        }
        [Fact]
        public async Task GetProductsByBatch()
        {
            var responses = await odata.BatchAsync(new BatchRequest($"{endpoint}/$batch")
            {
                Requests = new [] {Request.Get($"{endpoint}/Products")}
            });

            Assert.True(responses.All(res => res.Success));

            var products = responses.First().ReadAs<IEnumerable<dynamic>>("$.value");

            Assert.NotNull(products);
            Assert.True(products.Count() > 0);
        }
        [Fact]
        public async Task GetProduct()
        {
            var response = await odata.SendAsync(Request.Get($"{endpoint}/Products(@Id)", new { Id = 1 }));

            Assert.True(response.Success);

            var product = response.ReadAs<dynamic>();

            Assert.NotNull(product);
            Assert.Equal(1, (int)product.ID);
        }
        [Fact]
        public async Task ChangeProductName()
        {
            var searched = await odata.SendAsync(Request.Get($"{endpoint}/Products?$top=1"));

            Assert.True(searched.Success);

            var product = searched.ReadAs<dynamic>("$.value[0]");

            Assert.NotNull(product);

            var uri = $"{endpoint}/Products({product.ID})";
            string name = product.Name;
            var patch = new { Name = Guid.NewGuid().ToString() };

            var changed = await odata.SendAsync(Request.Patch(uri, patch, type: "ODataDemo.Product", typeKey: "@odata.type"));

            Assert.True(changed.Success);
            Assert.Null(changed.ReadAs<object>());

            var lookuped = await odata.SendAsync(Request.Get($"{uri}/Name/$value"));

            Assert.True(lookuped.Success);
            
            var replacedName = lookuped.ReadAs<string>();

            Assert.NotEqual(name, replacedName);
            Assert.Equal(patch.Name, replacedName);

        }
        [Fact]
        public async Task CreateProductWithCategory()
        {
            var batch = new BatchRequest($"{endpoint}/$batch")
            {
                Requests =
                {
                    Request.Post($"{endpoint}/Products", new 
                    { 
                        ID = 100,
                        Name = "Test Product", 
                        Description = "Test Item", 
                        ReleaseDate = DateTime.Now,
                        Rating = 1,
                        Price = 1.0,
                    }, type:"ODataDemo.Product", typeKey: "@odata.type"),
                    Request.Post($"{endpoint}/$1/Categories/$ref", 
                        new Dictionary<string, string> {
                            { "@odata.id", $"{endpoint}/Categories(0)" }
                        }),
                    Request.Get($"{endpoint}/Products")
                }
            };

            var responses = await odata.SendAsync(batch);
            
            Assert.Equal(batch.Requests.Count(), responses.Count());
            
            foreach(var response in responses)
                Assert.True(response.Success);

            var id = responses.First().ReadAs<int>("$.ID");
            var deleted = await odata.SendAsync(Request.Delete($"{endpoint}/Products({id})"));

            Assert.True(deleted.Success);
        }

        [Fact]
        public async Task GetNullWhenNotFound()
        {
            var response = await odata.SendAsync(Request.Get($"{endpoint}/Products(-1)"));

            Assert.True(response.Success);

            var product = response.ReadAs<IEnumerable<dynamic>>();

            Assert.Null(product);
        }

        [Fact]
        public async Task SearchByDate()
        {
            var response = await odata.SendAsync(Request.Get($"{endpoint}/Products?$filter=ReleaseDate gt @Date", new { Date = DateTimeOffset.Parse("1992-01-01T00:00:00Z") }));

            Assert.True(response.Success);

            var products = response.ReadAs<IEnumerable<dynamic>>("$.value");

            Assert.True(products.Count() > 0);
        }
    }
}
