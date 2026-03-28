using System;
using System.Linq;
using GamingGearBackend.Data;
using GamingGearBackend.Models;

namespace GamingGearBackend.Jobs
{
    public class ScheduledJobs
    {
        private readonly AppDbContext _db;

        public ScheduledJobs(AppDbContext db)
        {
            _db = db;
        }

        // Job 1: Tự động hủy đơn hàng quá 24h chưa thanh toán
        public void CancelExpiredOrders()
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-24);
            
            // Find all pending online payment orders (not VNPay/MoMo) older than 24h
            var expiredOrders = _db.Orders
                .Where(o => o.Status == "Chờ thanh toán" && o.CreatedAt < cutoffTime)
                .ToList();

            if (!expiredOrders.Any()) return;

            foreach (var order in expiredOrders)
            {
                order.Status = "Đã hủy";
                order.CancelledAt = DateTime.UtcNow;

                // Load order items to restore stock
                var orderItems = _db.OrderItems.Where(oi => oi.OrderId == order.Id).ToList();
                foreach (var item in orderItems)
                {
                    var product = _db.Products.Find(item.ProductId);
                    if (product != null && !product.IsPreOrder && !product.IsOrderOnly)
                    {
                        product.Stock += item.Quantity; // Restore stock
                    }
                }

                // Add notification
                _db.Notifications.Add(new Notification
                {
                    UserId = order.UserId,
                    Message = $"Đơn hàng #{order.Id} đã bị hủy do chưa thanh toán trong 24h",
                    CreatedAt = DateTime.UtcNow
                });
            }

            _db.SaveChanges();
            Console.WriteLine($"[Hangfire] Cancelled {expiredOrders.Count} expired orders.");
        }

        // Job 2: Tự động kích hoạt Flash Sale theo lịch
        public void ActivateFlashSale() { }

        // Job 3: Dọn dẹp token hết hạn trong DB mỗi ngày
        public void CleanExpiredTokens() { }
    }
}
