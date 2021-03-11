using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using StorageManageService.WebApi.Models;

namespace StorageManageService.WebApi.Tests
{
    public class TestServerFixture : IDisposable
    {
        public TestServer TestServer { get; }
        public StorageContext StorageContext { get; }
        public HttpClient HttpClient { get; }

        public TestServerFixture()
        {
            TestServer = new TestServer(new WebHostBuilder()
                .UseEnvironment("Testing")
                .UseStartup<Startup>());

            StorageContext = TestServer.Host.Services.GetService<StorageContext>();
            FillTestDb();

            HttpClient = TestServer.CreateClient();
        }

        private void FillTestDb()
        {
            StorageContext.Database.EnsureDeleted();
            StorageContext.Database.EnsureCreated();

            for (int i = 0; i < 3; i++)
            {
                var product = new Product
                {
                    Name = $"Product {i}"
                };

                StorageContext.Products.Add(product);
            }

            StorageContext.SaveChanges();

            for (int i = 0; i < 3; i++)
            {
                var storage = new Storage
                {
                    Name = $"Storage {i}"
                };

                StorageContext.Storages.Add(storage);
            }

            StorageContext.SaveChanges();

            var product1 = StorageContext.Products.Find(1);
            var product2 = StorageContext.Products.Find(2);

            var storage1 = StorageContext.Storages.Find(1);
            var storage2 = StorageContext.Storages.Find(2);

            storage1.StorageProducts.Add(new StorageProduct
            {
                Storage = storage1,
                Product = product1,
                Quantity = 1
            });
            
            storage2.StorageProducts.Add(new StorageProduct
                {
                    Storage = storage2,
                    Product = product1,
                    Quantity = 2
                });
            storage2.StorageProducts.Add(new StorageProduct
                {
                    Storage = storage2,
                    Product = product2,
                    Quantity = 3
                });

            StorageContext.SaveChanges();
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            TestServer.Dispose();
        }
    }
}
