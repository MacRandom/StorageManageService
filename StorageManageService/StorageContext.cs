using Microsoft.EntityFrameworkCore;
using StorageManageService.WebApi.Models;

namespace StorageManageService
{
    public class StorageContext : DbContext
    {
        public DbSet<Storage> Storages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StorageProduct> StorageProducts { get; set; }

        public StorageContext(DbContextOptions<StorageContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StorageProduct>()
                .HasKey(t => new { t.StorageId, t.ProductId });
        }
    }
}
