using System;
using Microsoft.EntityFrameworkCore;

namespace SharedModels
{
    public class OrderLine
    {  
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }       

    }
}
