using System.Collections.Generic;

namespace StorageManageService.WebApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<StorageProduct> StorageProducts { get; set; } = new List<StorageProduct>();
    }
}
