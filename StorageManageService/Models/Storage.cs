using System.Collections.Generic;

namespace StorageManageService.WebApi.Models
{
    public class Storage
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<StorageProduct> StorageProducts { get; set; } = new List<StorageProduct>();
    }
}
