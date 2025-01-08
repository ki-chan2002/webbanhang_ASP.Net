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
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, ReferenceHandler = ReferenceHandler.IgnoreCycles };
                var json = JsonSerializer.Serialize(messages, options);
                return Content(json, "application/json");
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
        public async Task<IActionResult> MarkMessageAsDone(int MessageID)
        {
            try
            {
                var message = await _context.Messages.FindAsync(MessageID); // Tìm message theo ID
                if (message == null)
                {
                    return NotFound(new { message = "Message not found." }); // Trả về 404 nếu không tìm thấy
                }

                // Thực hiện logic đánh dấu là đã hoàn thành
                // Ví dụ: message.Status = "Done";
                message.Type = "Done";

                await _context.SaveChangesAsync();
                return Ok(new { message = "Message marked as done successfully." }); // Trả về 200 OK với thông báo
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while marking the message as done: " + ex.Message }); // Trả về 500 Internal Server Error với thông báo lỗi
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkMessageAsFail(int MessageID)
        {
            try
            {
                var message = await _context.Messages.FindAsync(MessageID);
                if (message == null)
                {
                    return NotFound(new { message = "Message not found." });
                }

                message.Type = "Fail";
                await _context.SaveChangesAsync();
                return Ok(new { message = "Message marked as fail successfully." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while marking the message as fail: " + ex.Message });
            }
        }
    }
}
