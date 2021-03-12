using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using OrderApi.Models;
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
            //db.SaveChanges();
            return newOrder;
        }

        void IRepository<Order>.Edit(Order entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        Order IRepository<Order>.Get(int id)
        {
            Order order = db.Orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
                order.Products = db.Products.Where(p => p.OrderId == order.Id).ToList();

            return order;
        }

        IEnumerable<Order> IRepository<Order>.GetAll()
        {
            List<Order> orderList = db.Orders.ToList();
            foreach (Order order in orderList)
            {
                order.Products = db.Products.Where(p => p.OrderId == order.Id).ToList();
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
                order.Products = db.Products.Where(p => p.OrderId == order.Id).ToList();
            }
            return orderList;
        }
    }
}
