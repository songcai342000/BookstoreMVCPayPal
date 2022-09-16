using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayPalPaymentIntergration.Data;
using PayPalPaymentIntergration.Models;

namespace PayPalPaymentIntergration.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var applicationDbContext = _context.Reservations.Include(r => r.Book).Include(r => r.Order).Where(w => w.Order.UserId == userId && w.Order.Status == "Unpaid");
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.Order)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId");
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,OrderId,BookId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChooseBook(int? bookId)
        {
            /*ar orderId = 0;
            if (orderId == null)
            {
                Order order = new Order { UserId = userId, Status = "Unpaid", OrderTime = DateTime.Now };
                var orders = _context.Orders.Where(o => o.UserId == order.UserId && o.Status == "Unpaid" && o.OrderTime > DateTime.Now.AddDays(-1)).Count();
                if (orders == 0) //if there is no unpaid order during the last day, create a new order 
                {
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                }
                orderId = _context.Orders.Where(o => o.UserId == order.UserId).Select(s => s.OrderId).OrderBy(i => i).Last();//get the orderId of the last unpaid order of the last day
            }*/
            if (bookId == null)
            {
                return NotFound("Something is wrong, please try again.");
            }
            var userId = _userManager.GetUserId(HttpContext.User);
            var ordersCount = _context.Orders.Where(o => o.UserId == userId && o.Status == "Unpaid" && o.OrderTime > DateTime.Now.AddDays(-1)).Count();
            Order order = new Order { UserId = userId, Status = "Unpaid", OrderTime = DateTime.Now }; //Create a new order
            if (ordersCount == 0) //if there is no unpaid order during the last day, create a new order 
            {
                _context.Add(order); //add the new order to database
                await _context.SaveChangesAsync();
            }
            var orderId = _context.Orders.Where(o => o.UserId == order.UserId).Select(s => s.OrderId).OrderBy(i => i).Last();//get the orderId of the last unpaid order of the last 24 hours
            Reservation reservation = new Reservation { OrderId = (int)orderId, BookId = (int)bookId }; //create reservation
            _context.Add(reservation);
            await _context.SaveChangesAsync();
            return View(reservation);
        }


        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Book)
                .Include(r => r.Order)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
