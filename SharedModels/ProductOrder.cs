using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedModels
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
