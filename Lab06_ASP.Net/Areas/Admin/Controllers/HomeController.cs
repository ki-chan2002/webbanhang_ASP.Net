using Lab06_ASP.Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Lab06_ASP.Net.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly MyDBContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(MyDBContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize, HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ListOrder()
        {
            var order = _dbContext.Orders.ToList();
            return Json(order);
        }
        public IActionResult CreateOrder(Order order)
        {
            if(ModelState.IsValid)
            {
                _dbContext.Orders.Add(order);
                _dbContext.SaveChanges();
                return Json(order);
            }
            return BadRequest(ModelState);
        }
        [HttpGet]
        public IActionResult ListCoffee()
        {
            var coffees = _dbContext.Coffees.ToList();
            return Json(coffees);
        }
        [HttpGet]
        public IActionResult GetCoffee(int id) 
        {
            var coffee = _dbContext.Coffees.FirstOrDefault(c => c.Id == id);
            return Json(coffee);
        }

        [HttpPost]
        public IActionResult Create(Coffee coffee)
        {
            if (ModelState.IsValid)
            {
                if (coffee.ImageFile != null)
                {
                    int maxId = 0;
                    string filename = "";
                    // Đổi tên file ảnh thành "Coffee_id"
                    if(_dbContext.Coffees.Count() == 0)
                    {
                        maxId = 1;
                        filename = "Coffee_" + maxId + Path.GetExtension(coffee.ImageFile.FileName);
                    }
                    else
                    {
                        maxId = _dbContext.Coffees.Max(c => c.Id);
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
                // Lưu Coffee vào cơ sở dữ liệu
                _dbContext.Coffees.Add(coffee);
                _dbContext.SaveChanges();

                return Json(coffee);
            }
            return BadRequest(ModelState);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var coffee = _dbContext.Coffees.FirstOrDefault(c => c.Id == id);
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

                _dbContext.Coffees.Remove(coffee);
                _dbContext.SaveChanges();
                return Json(coffee);
            }
            return BadRequest(ModelState);
        }
        public IActionResult Get(int id)
        {
            var coffee = _dbContext.Coffees.ToList().Find(c => c.Id == id);
            return Json(coffee);
        }
        [HttpPost]
        public IActionResult Edit(Coffee coffee)
        {
            if (ModelState.IsValid)
            {
                var existingCoffee = _dbContext.Coffees.FirstOrDefault(c => c.Id == coffee.Id);
                if (existingCoffee != null)
                {
                    existingCoffee.Name = coffee.Name;
                    existingCoffee.S_Price = coffee.S_Price;
                    existingCoffee.L_Price = coffee.L_Price;
                    existingCoffee.Type = coffee.Type;

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
                        string newFilename = "Coffee_" + existingCoffee.Id + Path.GetExtension(coffee.ImageFile.FileName);
                        string newPhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vendor/img", newFilename);
                        using (var stream = new FileStream(newPhysicalPath, FileMode.Create))
                        {
                            coffee.ImageFile.CopyTo(stream);
                        }

                        // Thay đổi giá trị ImagePath nếu đuôi file ảnh mới có thay đổi
                        
                            existingCoffee.ImagePath = newFilename;
                        
                    }

                    _dbContext.SaveChanges();

                    return Json(existingCoffee);
                }
            }

            return BadRequest(ModelState);
        }
    }
}
