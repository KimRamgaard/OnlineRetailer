using System;
using Microsoft.EntityFrameworkCore;

namespace SharedModels
{

    [Keyless]
    public class OrderLine
    {  
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }       

    }
}
