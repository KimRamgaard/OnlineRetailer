using System;
namespace OrderApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int ItemsReserved { get; set; }
        public Order Order { get; set; }

    }
}
