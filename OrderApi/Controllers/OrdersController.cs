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
        readonly IMessagePublisher messagePublisher;

        public OrdersController(IRepository<Order> repository,
            IMessagePublisher publisher,
            IServiceGateway<Product> gateway)
        {
            _repository = repository;
            messagePublisher = publisher;
            productServiceGateway = gateway;
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

            RestClient restClient = new RestClient();

            // Ask Customer service if Customer is valid
            // *** Pierre TODO *** 
            restClient.BaseUrl = new Uri("customers/" + order.CustomerId);
            var customerRequest = new RestRequest(order.CustomerId.ToString(), Method.GET);
            var customerResponse = restClient.Execute(customerRequest);
            var customer = JObject.Parse(customerResponse.Content);
            if (customerResponse.IsSuccessful)
            {
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
            restClient.BaseUrl = new Uri("orders/");
            var orderRequest = new RestRequest("?CustomerNo=" + customer.Value<String>("CustomerId") , Method.GET);
            //List<Order> customerOrders = restClient.Execute<List<Order>>(orderRequest);


            
            
            
         

            // Ask Product service if products are available
            if (ProductItemsAvailable(order))
            {
                try
                {
                    // Publish OrderStatusChangedMessage. If this operation
                    // fails, the order will not be created
                    messagePublisher.PublishOrderStatusChangedMessage(
                        order.CustomerId, order.OrderLines, "completed");

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
                if (orderLine.Quantity > orderedProduct.ItemsInStock)
                {
                    return false;
                }
            }
            return true;
        }

        // PUT orders/5/cancel
        // This action method cancels an order and publishes an OrderStatusChangedMessage
        // with topic set to "cancelled".
        [HttpPut("{id}/cancel")]
        public IActionResult Cancel(int id)
        {

            try
            {
                Order selectedOrder = _repository.Get(id);

                if (selectedOrder != null)
                {
                    messagePublisher.PublishOrderStatusChangedMessage(selectedOrder.CustomerId, selectedOrder.OrderLines, "cancelled");

                    selectedOrder.Status = Order.OrderStatus.cancelled;

                    _repository.Edit(selectedOrder);

                    return Ok("Order was cancelled");
                }

                else
                {
                    return NotFound("Error: Order could not be found");
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.Message + ex.InnerException : ex.Message);
            }
        }

        // PUT orders/5/ship
        // This action method ships an order and publishes an OrderStatusChangedMessage.
        // with topic set to "shipped".
        [HttpPut("{id}/ship")]
        public IActionResult Ship(int id)
        {
            try
            {
                Order selectedOrder = _repository.Get(id);

                if (selectedOrder != null)
                {
                    messagePublisher.PublishOrderStatusChangedMessage(selectedOrder.CustomerId, selectedOrder.OrderLines, "shipped");

                    selectedOrder.Status = Order.OrderStatus.shipped;

                    _repository.Edit(selectedOrder);

                    return Ok("Order was shipped");
                }

                else
                {
                    return NotFound("Error: Order could not be shipped");
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.Message + ex.InnerException : ex.Message);
            }
        }

    }
}
