using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayPalPaymentIntergration.Data;
using PayPalPaymentIntergration.Models;
using Microsoft.Extensions.Caching.Memory;

namespace PayPalPaymentIntergration.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _memoryCache;


        public BooksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _memoryCache = memoryCache;

        }

        // GET: Books 
        public async Task<IActionResult> Index(int? id, int currentPageIndex) //return booklist, but if id is not null, reserve a book first
        {
            try
            {
                if (id != null)
                {
                    await ReserveBookAsync((int)id);
                }

                var totalPageNumber = 0;
                var totalItemNumber = _context.Books.Count();
                if (totalItemNumber % 2 == 0) //2 items each page
                {
                    totalPageNumber = totalItemNumber / 2;
                }
                else
                {
                    totalPageNumber = totalItemNumber / 2 + 1;
                }
                var books = from b in _context.Books
                            join g in _context.Genres on b.GenreId equals g.GenreId //get the genre name of the books
                            select new BookWithGenreName
                            {
                                BookId = b.BookId,
                                Title = b.Title,
                                Introduction = b.Introduction,
                                Price = b.Price,
                                Genre = g.Name,
                                ImageUrl = b.ImageUrl,
                                CurrentPageIndex = currentPageIndex,
                                TotalPageNumber = totalPageNumber
                            };
                IEnumerable<BookWithGenreName> CurrentBooks = null;
                if (currentPageIndex == 0) //view the first two items by default
                {
                    CurrentBooks = await books.Take(2).ToListAsync();
                }
                else
                {
                    CurrentBooks = await books.Skip(2 * (currentPageIndex - 1)).Take(2).ToListAsync();
                }
              
                var cachedValue = _memoryCache.GetOrCreate(
                    "bookList" + currentPageIndex.ToString(),
                    cacheEntry =>
                    {
                        cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(1200);
                        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1800);
                        return CurrentBooks;
                    });
                return View(cachedValue);//show the booklist
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return NotFound("Something is wrong. Please come back later.");
        }

        // GET: Books 
        public async Task<IActionResult> Index2(int? id, int currentPageIndex) //return booklist, but if id is not null, reserve a book first
        {
            try
            {
                if (id != null)
                {
                    await ReserveBookAsync((int)id);
                }
                var totalPageNumber = 0;
                var totalItemNumber = _context.Books.Count();
                if (totalItemNumber % 2 == 0) //2 items each page
                {
                    totalPageNumber = totalItemNumber / 2;
                }
                else
                {
                    totalPageNumber = totalItemNumber / 2 + 1;
                }
                var books = from b in _context.Books
                            join g in _context.Genres on b.GenreId equals g.GenreId //get the genre name of the books
                            select new BookWithGenreName
                            {
                                BookId = b.BookId,
                                Title = b.Title,
                                Introduction = b.Introduction,
                                Price = b.Price,
                                Genre = g.Name,
                                ImageUrl = b.ImageUrl,
                                CurrentPageIndex = currentPageIndex,
                                TotalPageNumber = totalPageNumber
                            };
              
                IEnumerable<BookWithGenreName> CurrentBooks = null;
                if (currentPageIndex == 0)
                {
                    CurrentBooks = await books.Take(2).ToListAsync();
                }
                else
                {
                    CurrentBooks = await books.Skip(2 * (currentPageIndex - 1)).Take(2).ToListAsync();
                }
             
                var cachedValue = _memoryCache.GetOrCreate(
                    "bookList" + currentPageIndex.ToString(),
                    cacheEntry =>
                    {
                        cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(1200);
                        cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1800);
                        return CurrentBooks;
                    });
                return View(cachedValue);//show the booklist
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return NotFound("Something is wrong. Please come back later.");
        }

        public async Task<IActionResult> ReserveBookAsync(int bookId)
        {
            var orderId = 0;
            var orderIdSession = HttpContext.Session.GetString("OrderId");
            if (string.IsNullOrEmpty(orderIdSession))
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var orders = _context.Orders.Where(o => o.UserId == userId && o.Status == "Unpaid" && o.OrderTime > DateTime.Now.AddDays(-1));
                if (orders.Count() == 0) //if the user has no unpaid order less than one day old, create a new order 
                {
                    Order order = new Order { UserId = userId, Status = "Unpaid", OrderTime = DateTime.Now };
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                }
                orderId = _context.Orders.Where(o => o.UserId == userId && o.Status == "Unpaid" && o.OrderTime > DateTime.Now.AddDays(-1)).OrderBy(i => i).Last().OrderId;//get the orderId of the last unpaid order of the last day
                HttpContext.Session.SetString("OrderId", orderId.ToString());
            }
            else
            {
                orderId = Convert.ToInt32(orderIdSession);
            }
            Reservation reservation = new Reservation { OrderId = orderId, BookId = bookId };
            _context.Add(reservation);
            await _context.SaveChangesAsync();
            return null;
        }

     
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name"); 
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,Title,Introduction,GenreId,Price,ImageUrl")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId); //load the names of the genres
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Introduction,GenreId,Price,ImageUrl")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
