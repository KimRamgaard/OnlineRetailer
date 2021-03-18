using System;
using System.Collections.Generic;
using EasyNetQ;
using OrderApi.Models;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        IBus bus;

        public MessagePublisher(string connectionString)
        {
            bus = RabbitHutch.CreateBus(connectionString);
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        public void PublishOrderStatusChangedMessage(int? customerId, IEnumerable<OrderLine> orderLines, string topic)
        {
            var message = new OrderStatusChangedMessage
            {
                CustomerId = customerId,
                OrderLines = orderLines
            };

            bus.PubSub.Publish(message, topic);
        }
    }
}