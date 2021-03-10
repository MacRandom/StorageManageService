using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageManageService.WebApi.Models
{
    public class StorageProduct
    {
        [Key, Column(Order = 0)]
        public int StorageId { get; set; }
        [Key, Column(Order = 1)]
        public int ProductId { get; set; }
        public Storage Storage { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
