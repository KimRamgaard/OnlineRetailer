using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using SharedModels;

namespace OrderApi.Data
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly OrderApiContext db;

        public OrderRepository(OrderApiContext context)
        {
            db = context;
        }

        Order IRepository<Order>.Add(Order entity)
        {
            if (entity.Date == null)
                entity.Date = DateTime.Now;
            
            var newOrder = db.Orders.Add(entity).Entity;
            db.SaveChanges();
            return newOrder;
        }

        void IRepository<Order>.Edit(Order entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public Order Get(int id)
        {
            return db.Orders.Include(o => o.OrderLines).FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<Order> GetAll()
        {
            List<Order> orderList = db.Orders.ToList();
            foreach (Order order in orderList)
            {
                order.OrderLines = db.Products.Where(p => p.OrderId == order.Id).ToList();
            }
            return orderList;
        }

        void IRepository<Order>.Remove(int id)
        {
            var order = db.Orders.FirstOrDefault(p => p.Id == id);
            db.Orders.Remove(order);
            db.SaveChanges();
        }

        IEnumerable<Order> IRepository<Order>.GetByCustomer(int CustomerID)
        {
            List<Order> orderList = db.Orders.Where(o => o.CustomerId == CustomerID).ToList();
            foreach (Order order in orderList)
            {
                order.OrderLines = db.Products.Where(p => p.OrderId == order.Id).ToList();
            }
            return orderList;
        }
    }
}
