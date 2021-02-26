using CustomerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(CustomerApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Products
            if (context.Customers.Any())
            {
                return;   // DB has been seeded
            }

            List<Customer> customers = new List<Customer>
            {
                new Customer { CustomerId = 1, FullName = "Peder Hansen", Email = "PH@email.dk", Phone = "12345678", CreditStanding = 500, BillingAddress = "Gade 1B, Tyskland", ShippingAddress = "Gade 1B, Tyskland" },
                new Customer { CustomerId = 2, FullName = "Fredrik Hansen", Email = "FH@email.de", Phone = "12345678", CreditStanding = 200, BillingAddress = "Gade 1B, Tyskland", ShippingAddress = "Gade 1B, Tyskland" },
                new Customer { CustomerId = 3, FullName = "Louise Pedersen", Email = "LP@email.com", Phone = "12345678", CreditStanding = 0, BillingAddress = "Gade 1B, Tyskland", ShippingAddress = "Gade 1B, Tyskland" },
                new Customer { CustomerId = 4, FullName = "Frederika Laudsen", Email = "FL@email.dk", Phone = "12345678", CreditStanding = 5500, BillingAddress = "Gade 1B, Tyskland", ShippingAddress = "Gade 1B, Tyskland" },
                new Customer { CustomerId = 5, FullName = "Hans Peder", Email = "HP@email.net", Phone = "12345678", CreditStanding = 2500, BillingAddress = "Gade 1B, Tyskland", ShippingAddress = "Gade 1B, Tyskland" }
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();
        }
    }
}
