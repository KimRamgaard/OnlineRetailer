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
        public IEnumerable<Order> GetAllOrders()
        {
            return _repository.GetAll();
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

            // Call ProductApi to get the product ordered
            RestClient c = new RestClient();

            // Ask Customer service if Customer is valid
            c.BaseUrl = new Uri("https://localhost:5002/customers/");
            var customerRequest = new RestRequest(order.CustomerId.ToString(), Method.GET);
            var customerResponse = c.Execute(customerRequest);
            if (!customerResponse.IsSuccessful)
            {
                return BadRequest("Customer does not exist");
            }

            // Ask Product service if products are available
            c.BaseUrl = new Uri("https://localhost:5001/products/");
            var ProductRequest = new RestRequest(Method.PUT).AddJsonBody(order.Products);
            var ProductResponse = c.Execute(ProductRequest);
            if (ProductResponse.IsSuccessful)
            {
                var newOrder = _repository.Add(order);
                return CreatedAtRoute("GetOrder", new { id = newOrder.Id }, newOrder);
            }

            // If the order could not be created, "return no content".
            return NoContent();
        }
        

    }
}
