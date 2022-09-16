using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Models
{
    public class BookWithGenreName
    {
        [Key]
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public string Genre { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int CurrentPageIndex { get; set; }
        public int TotalPageNumber { get; set; }

    }
}
