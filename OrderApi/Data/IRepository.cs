using OrderApi.Models;
using System.Collections.Generic;

namespace OrderApi.Data
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();

        T Get(int id);
        T Add(T SharedEntity);
        void Edit(T entity);
        void Remove(int id);
        IEnumerable<Order> GetByCustomer(int CustomerID);
    }
}
