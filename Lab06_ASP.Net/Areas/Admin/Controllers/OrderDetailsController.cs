using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab06_ASP.Net.Models;

namespace Lab06_ASP.Net.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderDetailsController : Controller
    {
        private readonly MyDBContext _context;

        public OrderDetailsController(MyDBContext context)
        {
            _context = context;
        }

        // GET: Admin/OrderDetails
        public async Task<IActionResult> Index()
        {
            var myDBContext = _context.OrdersDetail.Include(o => o.Coffee).Include(o => o.Order);
            return View(await myDBContext.ToListAsync());
        }

        // GET: Admin/OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrdersDetail
                .Include(o => o.Coffee)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: Admin/OrderDetails/Create
        public IActionResult Create()
        {
            ViewData["CoffeeID"] = new SelectList(_context.Coffees, "CoffeeID", "CoffeeName");
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "CustomerEmail");
            return View();
        }

        // POST: Admin/OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDetailID,OrderID,CoffeeID,Quantity,Price,Size")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CoffeeID"] = new SelectList(_context.Coffees, "CoffeeID", "CoffeeName", orderDetail.CoffeeID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "CustomerEmail", orderDetail.OrderID);
            return View(orderDetail);
        }

        // GET: Admin/OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrdersDetail.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["CoffeeID"] = new SelectList(_context.Coffees, "CoffeeID", "CoffeeName", orderDetail.CoffeeID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "CustomerEmail", orderDetail.OrderID);
            return View(orderDetail);
        }

        // POST: Admin/OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailID,OrderID,CoffeeID,Quantity,Price,Size")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderDetailID))
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
            ViewData["CoffeeID"] = new SelectList(_context.Coffees, "CoffeeID", "CoffeeName", orderDetail.CoffeeID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "CustomerEmail", orderDetail.OrderID);
            return View(orderDetail);
        }

        // GET: Admin/OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrdersDetail
                .Include(o => o.Coffee)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: Admin/OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrdersDetail.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrdersDetail.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrdersDetail.Any(e => e.OrderDetailID == id);
        }
    }
}
