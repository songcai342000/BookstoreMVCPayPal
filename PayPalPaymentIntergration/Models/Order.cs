using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public virtual List<Reservation> Reservations { get; set; }
        public string Status { get; set; }
        public DateTime OrderTime { get; set; }

    }
}
