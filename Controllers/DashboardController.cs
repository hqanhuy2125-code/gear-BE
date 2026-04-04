using GamingGearBackend.Data;
using GamingGearBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly AppDbContext _db;

        public DashboardController(IDashboardService dashboardService, AppDbContext db)
        {
            _dashboardService = dashboardService;
            _db = db;
        }

        // Owner-only: full stats với biểu đồ doanh thu
        [HttpGet("stats")]
        [Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _dashboardService.GetStatsAsync();
            return Ok(stats);
        }

        // Admin: stats cơ bản để hiển thị trên dashboard
        [HttpGet("admin-stats")]
        [Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> GetAdminStats()
        {
            var totalOrders = await _db.Orders.CountAsync();
            var totalProducts = await _db.Products.CountAsync();
            var totalUsers = await _db.Users.CountAsync(u => u.Role == "customer");
            var pendingOrders = await _db.Orders.CountAsync(o => o.Status == "Pending" || o.Status == "Chờ xác nhận");
            var totalRevenue = await _db.Orders
                .Where(o => o.Status == "Delivered" || o.Status == "Đã giao")
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            return Ok(new
            {
                totalOrders,
                totalProducts,
                totalUsers,
                pendingOrders,
                totalRevenue
            });
        }
    }
}
