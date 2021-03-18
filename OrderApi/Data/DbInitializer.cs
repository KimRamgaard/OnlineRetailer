using System.Collections.Generic;
using System.Linq;
using SharedModels;
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
                OrderLines = new List<OrderLine>
                {
                    new OrderLine(){ProductId = 1, Quantity = 2},
                    new OrderLine(){ProductId = 2, Quantity = 3}
                }

            };

            context.Orders.Add(order);
            context.SaveChanges();
        }
    }
}
