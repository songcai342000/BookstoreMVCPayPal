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
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace PayPalPaymentIntergration.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,UserId,Status,OrderTime")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Checkout
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var orderId = 0;
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("OrderId")))
                {
                    orderId = Convert.ToInt32(HttpContext.Session.GetString("OrderId"));
                }
                else
                {
                    var orders = from i in _context.Orders where i.UserId == _userManager.GetUserId(HttpContext.User) && i.Status == "Unpaid" && i.OrderTime > DateTime.Now.AddDays(-1) select i;
                    if (orders == null || orders.Count() == 0)
                    {
                        return NotFound("You havn't chosen a book!");
                    }
                    orderId = orders.OrderBy(t => t.OrderTime).Select(d => d.OrderId).Last();
                }
                var books = (from o in _context.Orders join r in _context.Reservations on o.OrderId equals r.OrderId where o.OrderId == orderId select r).AsEnumerable().GroupBy(r => r.BookId); //must convert to enumerable to evaluate on client-side
                if (books == null)
                {
                    return NotFound("You haven't chosen any item yet!");
                }
                var cartItems = from bc in _context.Books.AsEnumerable()
                                join bs in books on bc.BookId equals bs.Key
                                select new CartItem
                                {
                                    Title = bc.Title,
                                    Price = bc.Price,
                                    ImageUrl = bc.ImageUrl,
                                    Quantity = bs.Count(),
                                    Total = bc.Price * bs.Count()
                                };
               

                await Task.Delay(500);
                return View(cartItems);
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
            return null;
        }

        //create excel -- not in use presently
       /* public static void CreateExcel(IEnumerable<CartItem> cartItems)
        {
            string excelPath = "E:\\repos\\OrderDetails.xls";
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            try
            {
                //create the schema
                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                string cellName;
                int counter = 1;
                foreach (var item in cartItems)
                {
                    cellName = "A" + counter.ToString();
                    var range = xlWorkSheet.get_Range(cellName, cellName);
                    range.Value2 = item.ToString();
                    ++counter;
                }
                //save the file
                xlWorkBook.SaveAs("", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlApp);
                releaseObject(xlWorkBook);
                releaseObject(xlWorkSheet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        //release office objects  -- not in use
        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: Orders/GetOrderId
        public async Task<IActionResult> PaymentSuccess(Status status)
        {
            try
            {
                var orderId = Convert.ToInt32(HttpContext.Session.GetString("OrderSession"));
                var order = await _context.Orders.FindAsync(orderId);
                order.Status = status.PaymentStatus;
                _context.Update(order);
                await _context.SaveChangesAsync();
                return View();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View("Something is wrong. Please come back later");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,UserId,Status,OrderTime")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return View(order);
        }

        [Authorize(Roles = "Admin")]
        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize(Roles = "Admin")]
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
