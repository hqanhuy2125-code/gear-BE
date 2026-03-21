using GamingGearBackend.Data;
using GamingGearBackend.DTOs;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrdersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll()
        {
            var orders = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("user/{userId:int}")]
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
        public async Task<ActionResult<Order>> Create([FromBody] CreateOrderDto dto)
        {
            // Validate user exists
            var user = await _db.Users.FindAsync(dto.UserId);
            if (user == null) return BadRequest(new { message = "User not found." });

            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(new { message = "Order must have at least one item." });

            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var order = new Order
            {
                UserId = dto.UserId,
                Status = "Pending",
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in dto.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    return BadRequest(new { message = $"Product {item.ProductId} not found." });

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.Price
                });
            }

            order.TotalPrice = order.OrderItems.Sum(x => x.Price * x.Quantity);

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // Re-load with includes to return full data
            var created = await _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstAsync(o => o.Id == order.Id);

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = dto.Status;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
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

