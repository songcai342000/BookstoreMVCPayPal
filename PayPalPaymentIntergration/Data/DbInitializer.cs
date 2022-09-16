using PayPalPaymentIntergration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any book.
            if (context.Books.Any())
            {
                return;   // DB has been seeded
            }
            var genres = new Genre[] {

                new Genre
                {
                    Name = "Fiction"
                },
                new Genre
                {
                    Name = "Music"
                }
            };
            foreach (Genre g in genres)
            {
                context.Genres.Add(g);
            }
            context.SaveChanges();
            var books = new Book[] {

                new Book
                {
                    Title = "Raghuvamsa Sudha",
                    Introduction = "The collection of the lyrics of the most popular of Sara Mingten",
                    GenreId = genres.Single(g => g.Name == "Fiction").GenreId,
                    Price = 199,
                    ImageUrl = "assets/askimgreen.png"
                },
                new Book
                {
                    Title = "Yellow roses",
                    Introduction = "An astronaut fant some flowers on Mars",
                    GenreId = genres.Single(g => g.Name == "Music").GenreId,
                    Price = 299,
                    ImageUrl = "assets/askimgred.png"
                }
            };
            foreach (Book b in books)
            {
                context.Books.Add(b);
            }
            context.SaveChanges();
            /*var customers = new Customer[] {

                new Customer
                {
                    Name = "Jun Li",
                    Email = "lijun@korkit.com",
                    Address = "Furuveien 13",
                    PostNumber = "1234",
                    Country = "USA",
                    Mobil = "98689898"
                }
            };
            foreach (Customer c in customers)
            {
                context.Customers.Add(c);
            }
            context.SaveChanges();*/
            /*var reservations = new Reservation[] {

               new Reservation
               {
                   OrderId = 1,
                   BookId = context.Books.Single(s => s.Title == "Raghuvamsa Sudha").BookId,
                   ReservationTime = DateTime.Now
               },
               new Reservation
               {
                   OrderId = 1,
                   BookId = context.Books.Single(s => s.Title == "Yellow roses").BookId,
                   ReservationTime = DateTime.Now
               },
               new Reservation
               {
                   OrderId = 2,
                   BookId = context.Books.Single(s => s.Title == "Raghuvamsa Sudha").BookId,
                   ReservationTime = DateTime.Now.AddDays(-1)
               }
           };
            foreach (Reservation r in reservations)
            {
                context.Reservations.Add(r);
            }
            context.SaveChanges();
            var orders = new Order[]
             {

               new Order
               {
                   CustomerId = context.Customers.Single(s => s.Email == "lijun@korkit.com").CustomerId,
                   Status = "Not Paid"
               },
               new Order
               {
                   CustomerId = context.Customers.Single(s => s.Email == "lijun@korkit.com").CustomerId,
                   Status = "Not Paid"
               }
           };
            foreach (Order o in orders)
            {
                context.Orders.Add(o);
            }
            context.SaveChanges();*/
        }

    }
}
