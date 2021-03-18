using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Models;
using RestSharp;


namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> _repository;

        //private readonly OrderApiContext _context;

        public OrdersController(IRepository<Order> repository)
        {
            _repository = repository;
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
            c.BaseUrl = new Uri("customers/");
            var customerRequest = new RestRequest(order.CustomerId.ToString(), Method.GET);
            var customerResponse = c.Execute(customerRequest);
            if (!customerResponse.IsSuccessful)
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
            // *** JAKOB TODO ***
            c.BaseUrl = new Uri("products");
            var ProductRequest = new RestRequest(Method.PUT).AddJsonBody(order.Products);
            var ProductResponse = c.Execute(ProductRequest);
            if (ProductResponse.IsSuccessful)
            {
                var newOrder = _repository.Add(order);
                return CreatedAtRoute("GetOrder", new { id = newOrder.Id }, newOrder);
            }
            else
            {
                // If the order could not be created, "return no content".
                return BadRequest(ProductResponse.ErrorMessage);
            }

            

            
        }
        

    }
}
