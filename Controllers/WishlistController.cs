using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/wishlist")]
    [Authorize(Policy = "CustomerOnly")]
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _db;

        public WishlistController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetWishlist(int userId)
        {
            var wishlist = await _db.WishlistItems
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .Select(w => w.Product)
                .ToListAsync();

            return Ok(wishlist);
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleWishlist([FromBody] WishlistToggleDto dto)
        {
            var userId = int.Parse(userIdStr);

            var user = await _db.Users.FindAsync(userId);
            if (user == null || user.Role != "customer")
            {
                return StatusCode(403, new { message = "Admin và Owner không có quyền sử dụng Wishlist." });
            }
            
            var existing = await _db.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == dto.ProductId);

            if (existing != null)
            {
                _db.WishlistItems.Remove(existing);
                await _db.SaveChangesAsync();
                return Ok(new { success = true, action = "removed" });
            }
            else
            {
                var item = new WishlistItem
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    CreatedAt = DateTime.UtcNow
                };
                _db.WishlistItems.Add(item);
                await _db.SaveChangesAsync();
                return Ok(new { success = true, action = "added" });
            }
        }

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = int.Parse(userIdStr);

            var user = await _db.Users.FindAsync(userId);
            if (user == null || user.Role != "customer")
            {
                return StatusCode(403, new { message = "Admin và Owner không có quyền sử dụng Wishlist." });
            }
            
            var existing = await _db.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existing == null)
            {
                var item = new WishlistItem
                {
                    UserId = userId,
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow
                };
                _db.WishlistItems.Add(item);
                await _db.SaveChangesAsync();
            }
            
            return Ok(new { success = true });
        }

        public class WishlistToggleDto
        {
            public int UserId { get; set; }
            public int ProductId { get; set; }
        }
    }
}
