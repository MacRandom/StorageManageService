using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageManageService.WebApi.Models;

namespace StorageManageService.WebApi.Controllers
{
    [Route("api/{controller}")]
    public class StorageController
    {
        private StorageContext _context;

        public StorageController(StorageContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var storages = await _context.Storages.Select(s => new
                {
                    s.Id,
                    s.Name
                }).ToListAsync();

                return new JsonResult(storages);
            }
            catch (Exception e)
            {
                return new ContentResult
                {
                    Content = e.Message
                };
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var storage = await _context.Storages.FirstOrDefaultAsync(s => s.Id == id);

                if (storage == null)
                    return new NotFoundResult();

                var storageObject = new
                {
                    storage.Id,
                    storage.Name,
                    Products = storage.StorageProducts.Select(sp => new
                    {
                        sp.Product.Id,
                        sp.Product.Name,
                        sp.Quantity
                    }).ToList()
                };

                return new JsonResult(storageObject);
            }
            catch (Exception e)
            {
                return new ContentResult
                {
                    Content = e.Message
                };
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddStorageAsync(string name)
        {
            try
            {
                var storage = new Storage
                {
                    Name = name
                };

                await _context.Storages.AddAsync(storage);

                await _context.SaveChangesAsync();

                return new OkResult();
            }
            catch (Exception e)
            {
                return new ContentResult
                {
                    Content = e.Message
                };
            }

        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdateStorageAsync(int id, string name)
        {
            try
            {
                var storage = await _context.Storages.FirstOrDefaultAsync(s => s.Id == id);

                if (storage == null)
                    return new NotFoundResult();

                storage.Name = name;

                await _context.SaveChangesAsync();

                return new OkResult();
            }
            catch (Exception e)
            {
                return new ContentResult
                {
                    Content = e.Message
                };
            }
        }

        [HttpPost]
        [Route("product")]
        public async Task<IActionResult> UpsertProductAsync(int storageId, int productId, int quantity)
        {
            try
            {
                var storageProduct = await _context.StorageProducts.FirstOrDefaultAsync(x => x.StorageId == storageId && x.ProductId == productId);

                if (storageProduct == null)
                {
                    var storage = await _context.Storages.FirstOrDefaultAsync(p => p.Id == storageId);
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

                    if (storage == null || product == null)
                        return new NotFoundResult();

                    storage.StorageProducts.Add(new StorageProduct
                    {
                        Storage = storage,
                        Product = product,
                        Quantity = quantity
                    });
                }
                else
                {
                    storageProduct.Quantity = quantity;
                }

                await _context.SaveChangesAsync();

                return new OkResult();
            }
            catch (Exception e)
            {
                return new ContentResult
                {
                    Content = e.Message
                };
            }

        }

        [HttpDelete]
        [Route("product")]
        public async Task<IActionResult> DeleteProductAsync(int storageId, int productId)
        {
            try
            {
                var storageProduct = await _context.StorageProducts.FirstOrDefaultAsync(x => x.StorageId == storageId && x.ProductId == productId);

                if (storageProduct == null)
                    return new NotFoundResult();

                _context.StorageProducts.Remove(storageProduct);

                await _context.SaveChangesAsync();

                return new OkResult();
            }
            catch (Exception e)
            {
                return new ContentResult
                {
                    Content = e.Message
                };
            }
        }
    }
}
