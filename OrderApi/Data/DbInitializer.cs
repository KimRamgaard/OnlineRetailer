using System.Collections.Generic;
using System.Linq;
using OrderApi.Models;
using System;

namespace OrderApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(OrderApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Products
            if (context.Orders.Any())
            {
                return;   // DB has been seeded
            }

            Order order = new Order()
            {
                Id = 1,
                CustomerId = 1,
                Date = DateTime.Today,
                Products = new List<Product>
                {
                    new Product(){Id = 1, ItemsReserved = 2},
                    new Product(){Id = 2, ItemsReserved = 3}
                }

            };

            context.Orders.Add(order);
            context.SaveChanges();
        }
    }
}
