using GiaoDienAdmin.Areas.Admin.Models;
using GiaoDienAdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace GiaoDienAdmin.Areas.Admin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        // Các DbSet cho các bảng khác trong ứng dụng của bạn
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employees> Employees { get; set; }

        // Cấu hình các quan hệ giữa các thực thể
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Đảm bảo Identity được cấu hình đúng
                                                // Các cấu hình quan hệ 1-N tiếp theo
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.Id);
        }
    }
}
