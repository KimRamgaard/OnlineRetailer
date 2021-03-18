using Microsoft.EntityFrameworkCore;
using OrderApi.Models;
using SharedModels;

namespace OrderApi.Data
{
    public class OrderApiContext : DbContext
    {
        public OrderApiContext(DbContextOptions<OrderApiContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure domain classes using modelBuilder here

            modelBuilder.Entity<OrderLine>()
                .HasKey(o => new { o.OrderId, o.ProductId });
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> Products { get; set; }
    }
}
