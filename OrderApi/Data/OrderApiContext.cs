using Microsoft.EntityFrameworkCore;
using OrderApi.Models;

namespace OrderApi.Data
{
    public class OrderApiContext : DbContext
    {
        public OrderApiContext(DbContextOptions<OrderApiContext> options) : base(options)
        { }
        
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<ProductOrder>().HasKey(p => p.Id);
            modelBuilder.Entity<Order>().HasMany<ProductOrder>(o => o.Products).WithOne(p => p.Order).IsRequired();
        }
        

        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> Products { get; set; }
    }
}
