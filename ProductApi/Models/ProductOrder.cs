using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApi.Models
{
    public class ProductOrder
    {
        public int Id { get; set; }
        public int ItemsReserved { get; set; }

        public int OrderId { get; set; }
    }
}
