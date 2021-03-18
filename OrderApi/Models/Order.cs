using System;
using System.Collections.Generic;

namespace OrderApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }

        public int CustomerId { get; set; }

        public OrderStatus Status { get; set; }

        public enum OrderStatus
        {
            cancelled,
            completed,
            shipped,
            paid
        }
        public ICollection<ProductOrder> Products { get; set; }
    }

}
