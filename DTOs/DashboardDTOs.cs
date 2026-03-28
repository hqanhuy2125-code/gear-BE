using System.Collections.Generic;

namespace GamingGearBackend.DTOs
{
    public class DashboardStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public List<MonthlyRevenueDto> ResvenueByMonth { get; set; } = new List<MonthlyRevenueDto>();
        public List<TopProductDto> TopProducts { get; set; } = new List<TopProductDto>();
        public OrderStatusCountDto OrderStatusCounts { get; set; } = new OrderStatusCountDto();
    }

    public class TopProductDto
    {
        public string Name { get; set; } = string.Empty;
        public int Sales { get; set; }
    }

    public class OrderStatusCountDto
    {
        public int Pending { get; set; }
        public int Shipping { get; set; }
        public int Delivered { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public string Month { get; set; } = string.Empty; // e.g. "T1", "T2"...
        public decimal Revenue { get; set; }
    }
}
