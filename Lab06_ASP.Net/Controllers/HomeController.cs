using Lab06_ASP.Net.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lab06_ASP.Net.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDBContext _dbContext;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MyDBContext dBContext)
        {
            _logger = logger;
            _dbContext = dBContext;
        }

        public IActionResult Index()
        {
            var coffee = _dbContext.Coffees.ToList();
            return View(coffee);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}