﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SharedModels;
using System;

namespace OrderApi.Data
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly OrderApiContext db;

        public OrderRepository(OrderApiContext context)
        {
            db = context;
        }

        Order IRepository<Models.Order>.Add(Order entity)
        {
            if (entity.Date == null)
                entity.Date = DateTime.Now;
            
            var newOrder = db.Orders.Add(entity).Entity;
            //db.SaveChanges();
            return newOrder;
        }

        void IRepository<Models.Order>.Edit(Models.Order entity)
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
            return db.Orders.ToList();
        }

        void IRepository<Models.Order>.Remove(int id)
        {
            var order = db.Orders.FirstOrDefault(p => p.Id == id);
            db.Orders.Remove(order);
            db.SaveChanges();
        }

        IEnumerable<Models.Order> IRepository<Models.Order>.GetByCustomer(int CustomerID)
        {
            List<Models.Order> orderList = db.Orders.Where(o => o.CustomerId == CustomerID).ToList();
            foreach (Models.Order order in orderList)
            {
                order.Products = db.Products.Where(p => p.OrderId == order.Id).ToList();
            }
            return orderList;
        }
    }
}
