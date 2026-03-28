using Microsoft.AspNetCore.SignalR;
using GamingGearBackend.Data;
using GamingGearBackend.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _db;

        public ChatHub(AppDbContext db)
        {
            _db = db;
        }

        // When a user sends a message to Admin
        public async Task SendMessageToAdmin(int userId, string message)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(message)) return;

            var chatMsg = new ChatMessage
            {
                SenderId = userId,
                Message = message,
                IsAdminSender = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.ChatMessages.Add(chatMsg);
            await _db.SaveChangesAsync();

            // Notify all Admins
            await Clients.Group("Admins").SendAsync("ReceiveMessage", chatMsg);
            // Also echo back to the user
            await Clients.Caller.SendAsync("ReceiveMessage", chatMsg);
        }

        // When an Admin sends a message to a User
        public async Task SendMessageToUser(int adminId, int userId, string message)
        {
            if (adminId <= 0 || userId <= 0 || string.IsNullOrWhiteSpace(message)) return;

            var chatMsg = new ChatMessage
            {
                SenderId = adminId,
                ReceiverId = userId,
                Message = message,
                IsAdminSender = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.ChatMessages.Add(chatMsg);
            await _db.SaveChangesAsync();

            // Notify the specific User
            await Clients.Group($"User_Chat_{userId}").SendAsync("ReceiveMessage", chatMsg);
            // Echo back to Admin
            await Clients.Caller.SendAsync("ReceiveMessage", chatMsg);
        }

        public async Task JoinChat(int userId, bool isAdmin)
        {
            if (isAdmin)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_Chat_{userId}");
            }
        }
    }
}
