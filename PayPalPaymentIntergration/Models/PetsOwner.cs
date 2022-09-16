using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Models
{
    public class PetsOwners
    {
        public string Owner { get; set; }
        public List<Pet> Pets { get; set; }

    }
}
