using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Models
{
    public class SmptSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public int SenderName { get; set; }
        public int SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
