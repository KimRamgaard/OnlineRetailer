using System;
using System.Text.Json.Serialization;

namespace OrderApi.Models
{
    public class ProductOrder
    {
        public int Id { get; set; }
        public int ItemsReserved { get; set; }

        public int OrderId { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }

    }
}
