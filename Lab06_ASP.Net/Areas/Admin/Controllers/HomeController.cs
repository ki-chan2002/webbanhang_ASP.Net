using Lab06_ASP.Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            // Lấy danh sách Categories
            var categories = _dbContext.Categories.ToList();

            // Lấy danh sách Coffees
            var coffees = _dbContext.Coffees.ToList();

            // Tạo một ViewModel để chứa cả hai danh sách
            var viewModel = new IndexViewModel
            {
                Categories = categories,
                Coffees = coffees
            };

            return View(viewModel);
        }
    }
}
