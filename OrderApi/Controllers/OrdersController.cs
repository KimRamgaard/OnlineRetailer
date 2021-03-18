using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrderApi.Data;
using SharedModels;
using OrderApi.Models;
using RestSharp;
using OrderApi.Infrastructure;


namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> _repository;
        readonly IMessagePublisher messagePublisher;

        public OrdersController(IRepository<Order> repository,
            IMessagePublisher publisher)
        {
            _repository = repository;
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
            List<Order> customerOrders = restClient.Execute<List<Order>>(orderRequest);


            
            
            
         



            // Make a request for the products from the product API
            restClient.BaseUrl = new Uri("Products/");
            var productRequest = new RestRequest(Method.PUT);
            productRequest.RequestFormat = DataFormat.Json;
            productRequest.AddJsonBody(order.OrderLines);
            var productResponse = restClient.Execute(productRequest);

            if (!productResponse.IsSuccessful)
            {
                return BadRequest(productResponse.ErrorException);
            }


            //Create order 
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
    }
}
