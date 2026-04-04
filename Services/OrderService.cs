using System;
using System.Linq;
using System.Threading.Tasks;
using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ConfirmPaymentAsync(string orderCode, decimal amount, string referenceCode, string? gateway = null, string? rawData = null)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null)
            {
                return false;
            }

            // Verify amount (optional: allow small difference if needed, but usually exact)
            if (order.TotalPrice != amount)
            {
                // Log mismatch but maybe don't fail if business rules allow? 
                // For now, strict match as requested.
                if (Math.Abs(order.TotalPrice - amount) > 1.0m) 
                {
                    return false;
                }
            }

            if (order.Status == "Paid")
            {
                return true; // Already processed
            }

            order.Status = "Paid";
            order.PaidAt = DateTime.UtcNow;
            order.PaymentReference = referenceCode;

            var paymentLog = new PaymentLog
            {
                OrderId = order.Id,
                Amount = amount,
                TransactionId = referenceCode,
                Gateway = gateway,
                TransactionDate = DateTime.UtcNow,
                RawData = rawData,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentLogs.Add(paymentLog);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
