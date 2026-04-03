using GamingGearBackend.Data;
using GamingGearBackend.DTOs;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using GamingGearBackend.Hubs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<OrderHub> _orderHub;

        public OrdersController(AppDbContext db, IHubContext<OrderHub> orderHub)
        {
            _db = db;
            _orderHub = orderHub;
        }

        [HttpGet]
        [Authorize(Policy = "AllUsers")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            IQueryable<Order> query = _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product);

            // Nếu không phải admin hoặc owner, chỉ trả về đơn hàng của chính họ
            if (role != "admin" && role != "owner")
            {
                if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
                var userId = int.Parse(userIdStr);
                query = query.Where(o => o.UserId == userId);
            }

            var orders = await query.OrderByDescending(o => o.Id).ToListAsync();
            return Ok(orders);
        }

        [HttpGet("user/{userId:int}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "CustomerOnly")]
        public async Task<ActionResult<IEnumerable<Order>>> GetByUser(int userId)
        {
            var orders = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        [Authorize(Policy = "AllUsers")]
        public async Task<ActionResult<Order>> Create([FromBody] CreateOrderDto dto)
        {
            try 
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(new { message = "User not authenticated." });
                var userId = int.Parse(userIdStr);

                // Validate user exists & Check role manually
                var user = await _db.Users.FindAsync(userId);
                if (user == null) return BadRequest(new { message = "User not found." });
                
                if (user.Role != "customer")
                {
                    return StatusCode(403, new { message = "Chỉ tài khoản Customer mới được phép đặt hàng. Admin và Owner không có quyền này." });
                }

                if (dto.Items == null || dto.Items.Count == 0)
                    return BadRequest(new { message = "Order must have at least one item." });

                var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
                var products = await _db.Products
                    .Where(p => p.Id > 0 && productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                string initialStatus = "Pending";
                if (!string.IsNullOrEmpty(dto.PaymentMethod))
                {
                    var pm = dto.PaymentMethod.ToLower();
                    if (pm.Contains("banking")) initialStatus = "Chờ thanh toán";
                    else if (pm.Contains("cod")) initialStatus = "Chờ xác nhận";
                    else if (pm.Contains("momo") || pm.Contains("vnpay")) initialStatus = "Đã thanh toán";
                }

                var order = new Order
                {
                    UserId = userId,
                    Status = initialStatus,
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber,
                    ShippingAddress = dto.ShippingAddress,
                    PaymentMethod = dto.PaymentMethod,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var item in dto.Items)
                {
                    if (!products.TryGetValue(item.ProductId, out var product))
                    {
                        product = new Product
                        {
                            Name = string.IsNullOrWhiteSpace(item.ProductName) ? "Unknown Product" : item.ProductName,
                            Price = item.Price > 0 ? item.Price : 500000,
                            ImageUrl = item.ImageUrl ?? "",
                            Description = "Auto-created",
                            Category = "Uncategorized",
                            Stock = 100,
                            CreatedAt = DateTime.UtcNow
                        };
                        _db.Products.Add(product);
                        // Save here to get ID for next items if shared (though usually products are distinct in DTO)
                        await _db.SaveChangesAsync();
                        products[product.Id] = product;
                    }

                    if (!product.IsPreOrder && !product.IsOrderOnly)
                    {
                        if (product.Stock < item.Quantity)
                        {
                            return BadRequest(new { message = $"Sản phẩm '{product.Name}' không đủ tồn kho." });
                        }
                        product.Stock -= item.Quantity;
                    }

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price
                    });
                }

                if (dto.Items.Any(i => products.TryGetValue(i.ProductId, out var p) && p.IsOrderOnly))
                {
                    order.Status = "Đang xử lý order";
                }

                order.TotalPrice = order.OrderItems.Sum(x => x.Price * x.Quantity);

                // Voucher & Promotion Logic
                if (!string.IsNullOrEmpty(dto.VoucherCode))
                {
                    var voucher = await _db.Vouchers.FirstOrDefaultAsync(v => v.Code == dto.VoucherCode);
                    if (voucher != null && voucher.ExpiryDate > DateTime.UtcNow && (voucher.MaxUsages == 0 || voucher.UsedCount < voucher.MaxUsages))
                    {
                        if (voucher.Type == "percent")
                        {
                            order.TotalPrice -= order.TotalPrice * (voucher.Value / 100m);
                        }
                        else
                        {
                            order.TotalPrice -= voucher.Value;
                        }
                        if (order.TotalPrice < 0) order.TotalPrice = 0;
                        voucher.UsedCount++;
                    }
                }

                _db.Orders.Add(order);
                await _db.SaveChangesAsync(); // SAVE ORDER FIRST TO GET ID

                _db.Notifications.Add(new Notification
                {
                    UserId = userId,
                    Message = $"Đặt hàng thành công! Đơn hàng #{order.Id} đang được xử lý.",
                    CreatedAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync(); // SAVE NOTIFICATION

                var created = await _db.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .AsNoTracking() // Performance
                    .FirstAsync(o => o.Id == order.Id);

                return CreatedAtAction(nameof(GetById), new { id = order.Id }, created);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Order Creation Failed: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi hệ thống khi tạo đơn hàng.", detail = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = dto.Status;
            
            if (dto.Status == "Confirmed") order.ConfirmedAt = DateTime.UtcNow;
            else if (dto.Status == "Shipping") order.ShippingAt = DateTime.UtcNow;
            else if (dto.Status == "Delivered") order.DeliveredAt = DateTime.UtcNow;
            else if (dto.Status == "Cancelled") order.CancelledAt = DateTime.UtcNow;
            else if (dto.Status == "ProcessingOrder") order.Status = "Đang xử lý order";

            // Create notification for status update
            _db.Notifications.Add(new Notification
            {
                UserId = order.UserId,
                Message = $"Đơn hàng #{order.Id} đã chuyển sang trạng thái: {dto.Status}",
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            // Push real-time update via SignalR
            await _orderHub.Clients.Group($"User_{order.UserId}")
                .SendAsync("UpdateOrderStatus", new { OrderId = order.Id, Status = dto.Status });

            return NoContent();
        }

        [HttpPut("{id:int}/confirm-payment")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (order.Status != "Chờ thanh toán")
                return BadRequest(new { message = "Chỉ có thể xác nhận thanh toán cho đơn hàng đang chờ thanh toán." });

            order.Status = "Đã thanh toán";
            
            // Create notification for status update
            _db.Notifications.Add(new Notification
            {
                UserId = order.UserId,
                Message = $"Đơn hàng #{order.Id} đã được xác nhận thanh toán",
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            // Push real-time update via SignalR
            await _orderHub.Clients.Group($"User_{order.UserId}")
                .SendAsync("UpdateOrderStatus", new { OrderId = order.Id, Status = order.Status });

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

