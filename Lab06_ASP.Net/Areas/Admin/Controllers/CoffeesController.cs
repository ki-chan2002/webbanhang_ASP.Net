using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab06_ASP.Net.Models;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Lab06_ASP.Net.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoffeesController : Controller
    {
        private readonly MyDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public CoffeesController(MyDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Coffees
        public async Task<IActionResult> Index()
        {
            var myDBContext = _context.Coffees.Include(c => c.Category);
            return View(await myDBContext.ToListAsync());
        }

        // GET: Admin/Coffees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffee = await _context.Coffees
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.CoffeeID == id);
            if (coffee == null)
            {
                return NotFound();
            }

            return View(coffee);
        }

        // GET: Admin/Coffees/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Coffees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CoffeeID,CoffeeName,S_Price,L_Price,CategoryID,Description,ImagePath")] Coffee coffee)
        {
            if (ModelState.IsValid)
            {
                if (coffee.ImageFile != null)
                {
                    int maxId = 0;
                    string filename = "";
                    // Đổi tên file ảnh thành "Coffee_id"
                    if (_context.Coffees.Count() == 0)
                    {
                        maxId = 1;
                        filename = "Coffee_" + maxId + Path.GetExtension(coffee.ImageFile.FileName);
                    }
                    else
                    {
                        maxId = _context.Coffees.Max(c => c.CoffeeID);
                        filename = "Coffee_" + (maxId + 1) + Path.GetExtension(coffee.ImageFile.FileName);
                    }

                    // Lưu ảnh vào thư mục /vendor/img
                    string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vendor/img", filename);
                    using (var stream = new FileStream(physicalPath, FileMode.Create))
                    {
                        coffee.ImageFile.CopyTo(stream);
                    }

                    coffee.ImagePath = filename;
                }
                _context.Add(coffee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", coffee.CategoryID);
            return View(coffee);
        }

        // GET: Admin/Coffees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffee = await _context.Coffees.FindAsync(id);
            if (coffee == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", coffee.CategoryID);
            return View(coffee);
        }

        // POST: Admin/Coffees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CoffeeID,CoffeeName,S_Price,L_Price,CategoryID,Description,ImagePath")] Coffee coffee)
        {
            if (id != coffee.CoffeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (coffee.ImageFile != null)
                    {
                        // Xóa ảnh hiện có
                        string imagePath = coffee.ImagePath;
                        string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vendor/img", imagePath);
                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }

                        // Đổi tên và lưu ảnh mới
                        string newFilename = "Coffee_" + coffee.CoffeeID + Path.GetExtension(coffee.ImageFile.FileName);
                        string newPhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vendor/img", newFilename);
                        using (var stream = new FileStream(newPhysicalPath, FileMode.Create))
                        {
                            coffee.ImageFile.CopyTo(stream);
                        }

                        // Thay đổi giá trị ImagePath nếu đuôi file ảnh mới có thay đổi

                        coffee.ImagePath = newFilename;

                    }
                    _context.Update(coffee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoffeeExists(coffee.CoffeeID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", coffee.CategoryID);
            return View(coffee);
        }

        // GET: Admin/Coffees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffee = await _context.Coffees
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.CoffeeID == id);
            if (coffee == null)
            {
                return NotFound();
            }

            return View(coffee);
        }

        // POST: Admin/Coffees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coffee = await _context.Coffees.FindAsync(id);
            if (coffee != null)
            {
                // Lấy tên file ảnh từ ImagePath
                string imagePath = coffee.ImagePath;

                // Xóa ảnh từ thư mục /vendor/img
                string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vendor/img", imagePath);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                _context.Coffees.Remove(coffee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoffeeExists(int id)
        {
            return _context.Coffees.Any(e => e.CoffeeID == id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoffee(Coffee coffee)
        {
            if (ModelState.IsValid)
            {
                if (coffee.ImageFile != null && coffee.ImageFile.Length > 0)
                {
                    int maxId = 0;
                    string filename = "";
                    // Đổi tên file ảnh thành "Coffee_id"
                    if (_context.Coffees.Count() == 0)
                    {
                        maxId = 1;
                        filename = "Coffee_" + maxId + Path.GetExtension(coffee.ImageFile.FileName);
                    }
                    else
                    {
                        maxId = _context.Coffees.Max(c => c.CoffeeID);
                        filename = "Coffee_" + (maxId + 1) + Path.GetExtension(coffee.ImageFile.FileName);
                    }

                    // Lưu ảnh vào thư mục /vendor/img
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vendor/img", filename);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await coffee.ImageFile.CopyToAsync(fileStream);
                    }
                    coffee.ImagePath = filename; ;
                }
                _context.Add(coffee);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cà phê đã được thêm thành công." });
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine(string.Join("\n", errors)); // In lỗi ra console

            return Json(new { success = false, message = string.Join("<br/>", errors) });

        }

        public async Task<IActionResult> GetCoffee(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffee = await _context.Coffees.Include(c => c.Category).FirstOrDefaultAsync(m => m.CoffeeID == id);
            if (coffee == null)
            {
                return NotFound();
            }
            var coffeeDto = new
            {
                coffeeID = coffee.CoffeeID,
                coffeeName = coffee.CoffeeName,
                s_Price = coffee.S_Price,
                l_Price = coffee.L_Price,
                description = coffee.Description,
                categoryID = coffee.CategoryID,
                imagePath = coffee.ImagePath,
                //Nếu bạn cần cả tên category thì thêm vào đây
                categoryName = coffee.Category?.CategoryName
            };

            return Json(coffeeDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditCoffee(Coffee coffee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCoffee = _context.Coffees.FirstOrDefault(c => c.CoffeeID == coffee.CoffeeID);
                    if (existingCoffee != null)
                    {
                        existingCoffee.CoffeeName = coffee.CoffeeName;
                        existingCoffee.S_Price = coffee.S_Price;
                        existingCoffee.L_Price = coffee.L_Price;
                        existingCoffee.CategoryID = coffee.CategoryID;
                        existingCoffee.Description = coffee.Description;
                        if (coffee.ImageFile != null)
                        {
                            // Xóa ảnh hiện có
                            string imagePath = existingCoffee.ImagePath;
                            string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vendor/img", imagePath);
                            if (System.IO.File.Exists(physicalPath))
                            {
                                System.IO.File.Delete(physicalPath);
                            }

                            // Đổi tên và lưu ảnh mới
                            string newFilename = "Coffee_" + existingCoffee.CoffeeID + Path.GetExtension(coffee.ImageFile.FileName);
                            string newPhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vendor/img", newFilename);
                            using (var stream = new FileStream(newPhysicalPath, FileMode.Create))
                            {
                                coffee.ImageFile.CopyTo(stream);
                            }

                            // Thay đổi giá trị ImagePath nếu đuôi file ảnh mới có thay đổi

                            existingCoffee.ImagePath = newFilename;

                        }
                        _context.Update(existingCoffee);
                        await _context.SaveChangesAsync();
                        return Json(new { success = true, message = "Cà phê đã được cập nhật thành công." });
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoffeeExists(coffee.CoffeeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = string.Join("<br/>", errors) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoffee(int id)
        {
            var coffee = await _context.Coffees.FindAsync(id);
            if (coffee == null)
            {
                return Json(new { success = false, message = "Không tìm thấy cà phê." });
            }

            try
            {
                // Lấy tên file ảnh từ ImagePath
                string imagePath = coffee.ImagePath;

                // Xóa ảnh từ thư mục /vendor/img
                string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vendor/img", imagePath);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                _context.Coffees.Remove(coffee);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa cà phê thành công." });
            }
            catch (DbUpdateException ex)
            {
                // Xử lý lỗi nếu có ràng buộc khóa ngoại (ví dụ: CoffeeOrder)
                return Json(new { success = false, message = "Không thể xóa cà phê này vì có liên kết đến dữ liệu khác." + ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra trong quá trình xóa." + ex.Message });
            }
        }

        public async Task<IActionResult> ShowCoffeeList()
        {
            var coffees = await _context.Coffees.Include(c => c.Category).ToListAsync();

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, ReferenceHandler = ReferenceHandler.IgnoreCycles };
            var json = JsonSerializer.Serialize(coffees, options);
            return Content(json, "application/json");
        }
    }
}
