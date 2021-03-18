using System.Collections.Generic;
using SharedModels;
using OrderApi.Models;

namespace OrderApi.Infrastructure
{
    public interface IMessagePublisher
    {
        void PublishOrderStatusChangedMessage(int? customerId, IEnumerable<OrderLine> orderLines, string topic);

        bool IsInStock(Order order);
    }
}