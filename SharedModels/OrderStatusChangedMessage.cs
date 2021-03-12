using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels
{
    public class OrderStatusChangedMessage
    {
        public int? CustomerId { get; set; }
        public IEnumerable<Order.OrderLine> OrderLines { get; set; }
    }
}
