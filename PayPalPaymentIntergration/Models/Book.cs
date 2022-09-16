using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public int GenreId { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual List<Reservation> Reservations { get; set; }


    }
}
