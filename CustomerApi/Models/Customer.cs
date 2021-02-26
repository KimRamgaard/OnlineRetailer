using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Models
{
    public class Customer
    {
        int CustomerId { get; set; }
        string FullName { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string BillingAddress { get; set; }
        string ShippingAddress { get; set; }
        double CreditStanding { get; set; }

    }
}
