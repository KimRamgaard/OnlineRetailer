using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrderApi.Data;
using SharedModels;
using RestSharp;
using OrderApi.Infrastructure;


namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> _repository;
        IServiceGateway<Product> productServiceGateway;
        IMessagePublisher messagePublisher;

        //private readonly OrderApiContext _context;

        public OrdersController(IRepository<Order> repository,
            IServiceGateway<Product> gateway,
            IMessagePublisher publisher)
        {
            _repository = repository;
            productServiceGateway = gateway;
            messagePublisher = publisher;
        }

        // GET: orders
        [HttpGet]
        public IActionResult GetAllOrders(int CustomerId, Order.OrderStatus orderstatus)
        {
            if(CustomerId == 0)
            {
                return new ObjectResult(_repository.GetAll());
            }
            else
            {
                return new ObjectResult(_repository.GetByCustomer(CustomerId));
            }
        }


        // GET orders/5
        [HttpGet("{id:int}", Name = "GetOrder")]
        public IActionResult GetOrder(int id)
        {
            var item = _repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }


        // POST orders
        [HttpPost]
        public IActionResult Post([FromBody]Order order)
        {
            
            if (order == null)
            {
                return BadRequest();
            }

            RestClient c = new RestClient();

            // Ask Customer service if Customer is valid
            // *** Pierre TODO *** 
            c.BaseUrl = new Uri("customers/" + order.customerId);
            var customerRequest = new RestRequest(order.customerId.ToString(), Method.GET);
            var customerResponse = c.Execute(customerRequest);
            if (customerResponse.IsSuccessful)
            {
                var customer = JObject.Parse(customerResponse.Content);
                if(customer is null)
                {
                    return StatusCode(404, "The entered customer doesn't seem to exist yet. Please select an existing/valid customer");
                }
            }
            else
            {
                return BadRequest(customerResponse.ErrorMessage);
            }

            


            //GET Credit standing from customer 
            //localhost:5000/orders/?CustomerNo=1
            c.BaseUrl = new Uri("orders/");
            var orderRequest = new RestRequest("?CustomerNo=" + customerResponse.Content.ToString(), Method.GET);
            var orderResponse = c.Execute(orderRequest);

            Console.WriteLine(orderResponse.ToString());


            
            // Ask Product service if products are available
            if (ProductItemsAvailable(order))
            {
                try
                {
                    // Publish OrderStatusChangedMessage. If this operation
                    // fails, the order will not be created
                    messagePublisher.PublishOrderStatusChangedMessage(
                        order.customerId, order.OrderLines, "completed");

                    // Create order.
                    order.Status = Order.OrderStatus.completed;
                    var newOrder = _repository.Add(order);
                    return CreatedAtRoute("GetOrder", new { id = newOrder.Id }, newOrder);
                }
                catch
                {
                    return StatusCode(500, "An error happened. Try again.");
                }
            }
            else
            {
                // If there are not enough product items available.
                return StatusCode(500, "Not enough items in stock.");
            }
        }

        private bool ProductItemsAvailable(Order order)
        {
            foreach (var orderLine in order.OrderLines)
            {
                // Call product service to get the product ordered.
                var orderedProduct = productServiceGateway.Get(orderLine.ProductId);
                if (orderLine.Quantity > orderedProduct.ItemsInStock - orderedProduct.ItemsReserved)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
