using System.Collections.Generic;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public interface IMessagePublisher
    {
        void PublishOrderStatusChangedMessage(int customerId, IEnumerable<Order.OrderLine> orderLines, string topic);

        bool IsInStock(Order order);
    }
}