using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ChatController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("history/{userId:int}")]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetHistory(int userId)
        {
            var messages = await _db.ChatMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId || (m.IsAdminSender && m.ReceiverId == userId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("admin/users")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsersWithMessages()
        {
            // Get unique users who have sent or received messages
            var userIds = await _db.ChatMessages
                .Select(m => m.IsAdminSender ? m.ReceiverId : m.SenderId)
                .Where(id => id != null)
                .Distinct()
                .ToListAsync();

            var users = await _db.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new 
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    LastMessage = _db.ChatMessages
                        .Where(m => m.SenderId == u.Id || m.ReceiverId == u.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.Message)
                        .FirstOrDefault(),
                    LastTime = _db.ChatMessages
                        .Where(m => m.SenderId == u.Id || m.ReceiverId == u.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.CreatedAt)
                        .FirstOrDefault(),
                    UnreadCount = _db.ChatMessages
                        .Count(m => m.SenderId == u.Id && !m.IsRead && !m.IsAdminSender)
                })
                .OrderByDescending(u => u.LastTime)
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("read/{userId:int}")]
        public async Task<IActionResult> MarkAsRead(int userId)
        {
            var unread = await _db.ChatMessages
                .Where(m => m.SenderId == userId && !m.IsRead && !m.IsAdminSender)
                .ToListAsync();

            foreach (var m in unread) m.IsRead = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
