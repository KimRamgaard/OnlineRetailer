using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using SharedModels;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> repository;

        public ProductsController(IRepository<Product> repos)
        {
            repository = repos;
        }

        // GET products
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return repository.GetAll();
        }

        // GET products/5
        [HttpGet("{id}", Name="GetProduct")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST products
        [HttpPost]
        public IActionResult Post([FromBody]Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var newProduct = repository.Add(product);

            return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        // PUT products/5
        [HttpPut]

        public IActionResult PutProduct(List<ProductOrder> productOrders)
        {
            List<Product> products = new List<Product>();
            
            //iterate through products and check if stock is present
            foreach (ProductOrder productOrder in productOrders)
            {
                Product product = repository.Get(productOrder.Id);
                if (product is null)
                {
                    return BadRequest(); // Product does not exists
                }

                if(product.ItemsInStock <= productOrder.ItemsReserved)
                {
                    return BadRequest(); //not enough items in stock
                }
               
                //minus stock and save to apply later
                product.ItemsInStock -= productOrder.ItemsReserved;
                products.Add(product);

            }

            //Commit to database
            foreach (Product p in products)
            {
                repository.Edit(p);
            }

            return Ok(products);

        }

        // DELETE products/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (repository.Get(id) == null)
            {
                return NotFound();
            }

            repository.Remove(id);
            return new NoContentResult();
        }
    }
}
