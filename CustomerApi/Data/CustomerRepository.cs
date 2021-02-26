using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Data
{
    public class CustomerRepository : IRepository<Customer>
    {

        private readonly CustomerApiContext db;

        public CustomerRepository(CustomerApiContext context)
        {
            db = context;
        }

        Customer IRepository<Customer>.Add(Customer entity)
        {
            var newCustomer = db.Customers.Add(entity).Entity;
            db.SaveChanges();
            return newCustomer;
        }

        void IRepository<Customer>.Edit(Customer entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        Customer IRepository<Customer>.Get(int id)
        {
            return db.Customers.FirstOrDefault(c => c.CustomerId == id);
        }

        void IRepository<Customer>.Remove(int id)
        {
            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == id);
            db.Customers.Remove(customer);
            db.SaveChanges();
        }
    }
}
