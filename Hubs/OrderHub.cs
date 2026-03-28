using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GamingGearBackend.Hubs
{
    public class OrderHub : Hub
    {
        // Join a group for a specific user to receive their order updates
        public async Task JoinUserGroup(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        // Admins join a general group to see all updates (optional for now)
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
    }
}
