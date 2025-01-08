using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab06_ASP.Net.Models;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Lab06_ASP.Net.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MessagesController : Controller
    {
        private readonly MyDBContext _context;

        public MessagesController(MyDBContext context)
        {
            _context = context;
        }

        // GET: Admin/Messages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Messages.ToListAsync());
        }

        // GET: Admin/Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Admin/Messages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageID,CustomerName,CustomerEmail,Description,CustomerPhoneNumber,Type")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: Admin/Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        // POST: Admin/Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageID,CustomerName,CustomerEmail,Description,CustomerPhoneNumber,Type")] Message message)
        {
            if (id != message.MessageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageID))
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
            return View(message);
        }

        // GET: Admin/Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Admin/Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageID == id);
        }

        public async Task<IActionResult> ShowMessageList()
        {
            try
            {

                var messages = await _context.Messages.ToListAsync(); // Lấy danh sách Messages từ database
                return Json(messages);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching messages: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([Bind("MessageID,CustomerName,CustomerEmail,Description,CustomerPhoneNumber,Type")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return Json(message);
            }
            return BadRequest(ModelState);

        }

        [HttpPost]
        public async Task<IActionResult> SearchMessageByNameOrID([FromForm] string searchTerm) // Nhận searchTerm từ form
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest(new { message = "Vui lòng nhập từ khóa tìm kiếm." });
            }

            try
            {
                var messages = await _context.Messages
                    .Where(m => m.CustomerName.Contains(searchTerm) || m.MessageID.ToString().Contains(searchTerm))
                    .ToListAsync();

                if (messages == null || messages.Count == 0)
                {
                    return Ok(new List<object>()); // Trả về mảng rỗng nếu không tìm thấy
                }
                return Ok(messages); // Trả về kết quả tìm kiếm
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi tìm kiếm: {ex.Message}" });
            }
        }
    }
}

