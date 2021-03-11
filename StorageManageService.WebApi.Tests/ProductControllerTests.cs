using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StorageManageService.WebApi.Models;
using Xunit;

namespace StorageManageService.WebApi.Tests
{
    public class ProductControllerTests : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;
        private readonly HttpClient _httpClient;
        private readonly StorageContext _context;

        public ProductControllerTests(TestServerFixture fixture)
        {
            _fixture = fixture;
            _httpClient = _fixture.HttpClient;
            _context = _fixture.StorageContext;
        }

        [Fact]
        public async Task Get_ShouldReturnListResult()
        {
            var response = await _httpClient.GetAsync("/api/product");

            response.EnsureSuccessStatusCode();

            var models = JsonConvert.DeserializeObject<IEnumerable<Product>>(await response.Content.ReadAsStringAsync());

            Assert.NotEmpty(models);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Get_ShouldReturnProductWithCorrectId(int id)
        {
            var response = await _httpClient.GetAsync($"/api/product/{id}");

            response.EnsureSuccessStatusCode();

            var product = JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync());

            Assert.Equal(id, product.Id);
        }

        [Theory]
        [InlineData("Test Product")]
        public async Task Post_DbContainsNewProduct(string name)
        {
            var response = await _httpClient.PostAsync($"/api/product/add?name={name}", null);

            response.EnsureSuccessStatusCode();

            var product = _context.Products.Where(p => p.Name == name).FirstOrDefault();

            Assert.NotNull(product);
        }
        
        [Theory]
        [InlineData(1, "Product 4")]
        public async Task Post_ProductUpdated(int id, string name)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            var oldName = product.Name;

            var response = await _httpClient.PostAsync($"/api/product/update?id={id}&name={name}", null);

            response.EnsureSuccessStatusCode();

            await _context.Entry(product).ReloadAsync();

            Assert.NotEqual(oldName, product.Name);
            Assert.Equal(name, product.Name);
        }
    }
}
