using System;
using System.Threading;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Data;
using ProductApi.Models;
using SharedModels;

namespace ProductApi.Infrastructure
{
    public class MessageListener
    {
        IServiceProvider provider;
        string connectionString;

        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        public void Start()
        {
            using (var bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.PubSub.Subscribe<OrderStatusChangedMessage>("productAPIHkCompleted",
                    HandleOrderCompleted, x => x.WithTopic("completed"));

                // Add other subscribtions.





                lock (this)
                {
                    Monitor.Wait(this);
                }
            }
        }

        private void HandleOrderCompleted(OrderStatusChangedMessage message)
        {

        }
    }
}
