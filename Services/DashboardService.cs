using GamingGearBackend.Data;
using GamingGearBackend.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingGearBackend.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _db;

        public DashboardService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardStatsDto> GetStatsAsync()
        {
            var now = DateTime.UtcNow;
            var currentYear = now.Year;

            // 1. Basic Stats
            var stats = new DashboardStatsDto
            {
                TotalRevenue = await _db.Orders
                    .Where(o => o.Status == "Delivered")
                    .SumAsync(o => o.TotalPrice),
                
                TotalOrders = await _db.Orders.CountAsync(),
                TotalProducts = await _db.Products.CountAsync(),
                TotalUsers = await _db.Users.CountAsync()
            };

            // 2. Revenue by Month (Current Year)
            var monthlyData = await _db.Orders
                .Where(o => o.Status == "Delivered" && o.CreatedAt.Year == currentYear)
                .GroupBy(o => o.CreatedAt.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .ToListAsync();

            // Fill all 12 months
            for (int i = 1; i <= 12; i++)
            {
                var monthData = monthlyData.FirstOrDefault(m => m.Month == i);
                stats.ResvenueByMonth.Add(new MonthlyRevenueDto
                {
                    Month = $"T{i}",
                    Revenue = monthData?.Revenue ?? 0
                });
            }

            // 3. Top 3 Products (Current Month)
            var currentMonth = now.Month;
            stats.TopProducts = await _db.OrderItems
                .Where(oi => oi.Order.CreatedAt.Month == currentMonth && oi.Order.CreatedAt.Year == currentYear)
                .GroupBy(oi => oi.Product.Name)
                .Select(g => new TopProductDto
                {
                    Name = g.Key,
                    Sales = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(p => p.Sales)
                .Take(3)
                .ToListAsync();

            // 4. Order Status Counts
            var statusCounts = await _db.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            // Map and handle potential status naming variations
            // Note: In OrdersController, we use "Pending", "Chờ xác nhận", "Shipping", "Delivered", "Cancelled", "Đã thanh toán", "Đang xử lý order", "Chờ thanh toán"
            stats.OrderStatusCounts.Pending = statusCounts.Where(s => s.Status == "Pending" || s.Status == "Chờ xác nhận" || s.Status == "Chờ thanh toán").Sum(x => x.Count);
            stats.OrderStatusCounts.Shipping = statusCounts.Where(s => s.Status == "Shipping" || s.Status == "Đang giao").Sum(x => x.Count);
            stats.OrderStatusCounts.Delivered = statusCounts.Where(s => s.Status == "Delivered" || s.Status == "Đã giao").Sum(x => x.Count);

            return stats;
        }
    }
}
