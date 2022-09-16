using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PayPalPaymentIntergration.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayPalPaymentIntergration.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Genre> Genres { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Reservation>().HasOne<Order>(o => o.Order).WithMany(rs => rs.Reservations).HasForeignKey(o => o.OrderId);
            modelBuilder.Entity<Genre>().ToTable("Genres");
            modelBuilder.Entity<Book>().ToTable("Books"); ;
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<Reservation>().ToTable("Reservations");

        }
    }
}
