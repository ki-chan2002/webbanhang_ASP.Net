using Lab06_ASP.Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using static Lab06_ASP.Net.Models.SessionExtensions;

namespace Lab06_ASP.Net.Controllers
{
    public class CartController : Controller
    {

        private readonly MyDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CartController(MyDBContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _context = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Tải view index.cshtml
        }

        public IActionResult GetCart()
        {
            // Lấy dữ liệu giỏ hàng từ Session
            List<CartItem> cartItems = new List<CartItem>();
            var cartJson = HttpContext.Session.GetString("Cart");

            if (!string.IsNullOrEmpty(cartJson))
            {
                cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson) ?? new List<CartItem>();
            }

            // Trả về ViewModel với danh sách sản phẩm
            var cartViewModel = new CartViewModel
            {
                Values = cartItems // Đảm bảo đúng tên thuộc tính là "Values"
            };

            return Json(cartViewModel);
        }

        [HttpPost]
        public IActionResult AddToCart(int CoffeeID, string Size, decimal Price, int Quantity)
        {
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Tìm sản phẩm Coffee từ database
            var coffee = _context.Coffees.Find(CoffeeID);
            if (coffee == null)
            {
                return Json(new { success = false, message = "Coffee not found" });
            }

            // Kiểm tra nếu sản phẩm đã tồn tại trong giỏ hàng
            var existingItem = cart.FirstOrDefault(c => c.CoffeeID == CoffeeID && c.Size == Size);
            if (existingItem != null)
            {
                // Tăng số lượng nếu đã tồn tại
                existingItem.Quantity += Quantity;
            }
            else
            {
                // Thêm sản phẩm mới
                cart.Add(new CartItem
                {
                    CoffeeID = CoffeeID,
                    Coffee = coffee, // Ánh xạ đối tượng Coffee
                    Size = Size,
                    Price = Price,
                    Quantity = Quantity
                });
            }

            // Lưu lại giỏ hàng vào session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int CoffeeID, string Size, int Quantity)
        {
            try
            {
                var cartJson = HttpContext.Session.GetString("Cart");
                if (string.IsNullOrEmpty(cartJson))
                {
                    return Json(new { success = false, message = "Giỏ hàng trống." });
                }

                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
                if (cartItems == null)
                {
                    return Json(new { success = false, message = "Không thể đọc giỏ hàng." });
                }

                var itemToUpdate = cartItems.FirstOrDefault(item => item.CoffeeID == CoffeeID && item.Size == Size);
                if (itemToUpdate != null)
                {
                    itemToUpdate.Quantity = Quantity;
                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
                }
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi cập nhật số lượng: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int CoffeeID, string Size)
        {
            try
            {
                var cartJson = HttpContext.Session.GetString("Cart");
                List<CartItem> cartItems = new List<CartItem>();

                if (!string.IsNullOrEmpty(cartJson))
                {
                    cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
                }

                // Tìm sản phẩm cần xóa dựa trên CoffeeID và Size
                var itemToRemove = cartItems.FirstOrDefault(item => item.CoffeeID == CoffeeID && item.Size == Size);

                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);

                    // Lưu giỏ hàng đã cập nhật vào Session
                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));

                    return Json(new { success = true, message = "Sản phẩm đã được xóa." });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
                }
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xóa sản phẩm: {ex.Message}" });
            }
        }

    }


}
