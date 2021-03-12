using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StorageManageService.WebApi.Models;
using Xunit;

namespace StorageManageService.WebApi.Tests
{
    public class StorageControllerTests : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;
        private readonly HttpClient _httpClient;
        private readonly StorageContext _context;

        public StorageControllerTests(TestServerFixture fixture)
        {
            _fixture = fixture;
            _httpClient = _fixture.HttpClient;
            _context = _fixture.StorageContext;
        }

        [Fact]
        public async Task Get_ShouldReturnListResult()
        {
            var response = await _httpClient.GetAsync("/api/storage");

            response.EnsureSuccessStatusCode();

            var models = JsonConvert.DeserializeObject<IEnumerable<Storage>>(await response.Content.ReadAsStringAsync());

            Assert.NotEmpty(models);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Get_ShouldReturnStorageWithCorrectId(int id)
        {
            var response = await _httpClient.GetAsync($"/api/storage/{id}");

            response.EnsureSuccessStatusCode();

            var storage = JsonConvert.DeserializeObject<Storage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(id, storage.Id);
        }

        [Theory]
        [InlineData("Test Storage")]
        public async Task Post_DbContainsNewStorage(string name)
        {
            var response = await _httpClient.PostAsync($"/api/storage/add?name={name}", null);

            response.EnsureSuccessStatusCode();

            var storage = _context.Storages.Where(s => s.Name == name).FirstOrDefault();

            Assert.NotNull(storage);
        }

        [Theory]
        [InlineData(1, "Storage 3")]
        public async Task Post_StorageUpdated(int id, string name)
        {
            var storage = await _context.Storages.FirstOrDefaultAsync(p => p.Id == id);
            var oldName = storage.Name;

            var response = await _httpClient.PostAsync($"/api/storage/update?id={id}&name={name}", null);

            response.EnsureSuccessStatusCode();

            await _context.Entry(storage).ReloadAsync();

            Assert.NotEqual(oldName, storage.Name);
            Assert.Equal(name, storage.Name);
        }

        [Theory]
        [InlineData(1, 3, 3)]
        public async Task Post_StorageShouldContainAddedProduct(int storageId, int productId, int quantity)
        {
            var response = await _httpClient.PostAsync($"/api/storage/product?storageId={storageId}&productId={productId}&quantity={quantity}", null);

            var storageProduct = await _context.StorageProducts.Where(sp => sp.StorageId == storageId && sp.ProductId == productId).FirstOrDefaultAsync();

            response.EnsureSuccessStatusCode();

            Assert.NotNull(storageProduct);
            Assert.Equal(productId, storageProduct.ProductId);
            Assert.Equal(storageId, storageProduct.StorageId);
            Assert.Equal(quantity, storageProduct.Quantity);
        }
        
        [Theory]
        [InlineData(1, 1, 30)]
        public async Task Post_ProductQuantityShouldChange(int storageId, int productId, int quantity)
        {
            var storageProduct = await _context.StorageProducts.Where(sp => sp.StorageId == storageId && sp.ProductId == productId).FirstOrDefaultAsync();
            var oldQuantity = storageProduct.Quantity;

            var response = await _httpClient.PostAsync($"/api/storage/product?storageId={storageId}&productId={productId}&quantity={quantity}", null);

            response.EnsureSuccessStatusCode();

            await _context.Entry(storageProduct).ReloadAsync();

            Assert.NotEqual(oldQuantity, storageProduct.Quantity);
            Assert.Equal(quantity, storageProduct.Quantity);
        }
        
        [Theory]
        [InlineData(1, 1)]
        public async Task Post_ProductDeletedFromStorage(int storageId, int productId)
        {
            var response = await _httpClient.DeleteAsync($"/api/storage/product?storageId={storageId}&productId={productId}");

            response.EnsureSuccessStatusCode();

            var storageProduct = await _context.StorageProducts.Where(sp => sp.StorageId == storageId && sp.ProductId == productId).FirstOrDefaultAsync();

            Assert.Null(storageProduct);
        }
    }
}
