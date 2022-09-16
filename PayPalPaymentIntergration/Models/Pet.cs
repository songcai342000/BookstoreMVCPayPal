using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Models
{
    public class Pet
    {
        public string Name { get; set; }
        public Person Owner { get; set; }
    }
}
