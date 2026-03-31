using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReviewsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _db.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderBy(r => r.Id)
                .Select(r => new
                {
                    r.Id,
                    r.ProductId,
                    ProductName = r.Product != null ? r.Product.Name : "Sản phẩm không xác định",
                    ProductImage = r.Product != null ? r.Product.ImageUrl : "",
                    r.UserId,
                    UserName = r.User != null ? r.User.Name : "Người dùng ẩn",
                    r.NickName,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _db.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.ProductId,
                    r.UserId,
                    UserName = r.User.Name,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }

        public class CreateReviewDto
        {
            public int ProductId { get; set; }
            public int UserId { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; } = string.Empty;
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            // Kiểm tra user có mua sản phẩm chưa
            var hasPurchased = await _db.OrderItems
                .Include(oi => oi.Order)
                .AnyAsync(oi => oi.ProductId == dto.ProductId && oi.Order.UserId == dto.UserId && oi.Order.Status != "cancelled" && oi.Order.Status != "pending");

            if (!hasPurchased)
            {
                return BadRequest(new { message = "Bạn phải mua và nhận sản phẩm này mới được đánh giá." });
            }

            // Kiểm tra xem đã đánh giá chưa
            var existingReview = await _db.Reviews.FirstOrDefaultAsync(r => r.ProductId == dto.ProductId && r.UserId == dto.UserId);
            if (existingReview != null)
            {
                return BadRequest(new { message = "Bạn đã đánh giá sản phẩm này rồi." });
            }

            var review = new Review
            {
                ProductId = dto.ProductId,
                UserId = dto.UserId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Đánh giá thành công!" });
        }

        [HttpDelete("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            // Typically check if user is admin/owner here from JWT/session, but relying on frontend for simplicity right now
            // or pass user role in header if no JWT

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Đã xóa đánh giá" });
        }
    }
}
