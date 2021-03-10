using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageManageService.WebApi.Models;

namespace StorageManageService.WebApi.Controllers
{
    [Route("api/{controller}")]
    public class ProductController
    {
        private StorageContext _context;

        public ProductController(StorageContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var products = await _context.Products.Select(p => new
                {
                    p.Id,
                    p.Name
                }).ToListAsync();

                return new JsonResult(products);
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
                var product = await _context.Products.FindAsync(id);
                var productObject = new
                {
                    product.Id,
                    product.Name
                };

                return new JsonResult(productObject);
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
        public async Task<IActionResult> AddProductAsync(string name)
        {
            try
            {
                var product = new Product
                {
                    Name = name
                };

                await _context.Products.AddAsync(product);

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
        public async Task<IActionResult> UpdateProductAsync(int id, string name)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return new NotFoundResult();

                product.Name = name;

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
