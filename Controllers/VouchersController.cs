using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/vouchers")]
    public class VouchersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public VouchersController(AppDbContext db)
        {
            _db = db;
        }

        // Public: mọi user có thể GET để áp dụng voucher
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vouchers = await _db.Vouchers.OrderByDescending(v => v.CreatedAt).ToListAsync();
            return Ok(vouchers);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> GetById(int id)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();
            return Ok(voucher);
        }

        // Chỉ Owner tạo voucher
        [HttpPost]
        [Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> Create([FromBody] Voucher dto)
        {
            if (await _db.Vouchers.AnyAsync(v => v.Code == dto.Code))
            {
                return BadRequest(new { message = "Mã voucher đã tồn tại." });
            }

            dto.Id = 0;
            dto.CreatedAt = DateTime.UtcNow;
            dto.UsedCount = 0;
            _db.Vouchers.Add(dto);
            await _db.SaveChangesAsync();

            return Ok(dto);
        }

        // Chỉ Owner sửa voucher
        [HttpPut("{id:int}")]
        [Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] Voucher dto)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound(new { message = "Voucher không tồn tại." });

            voucher.Code = dto.Code;
            voucher.Type = dto.Type;
            voucher.Value = dto.Value;
            voucher.MinOrderValue = dto.MinOrderValue;
            voucher.ExpiryDate = dto.ExpiryDate;
            voucher.MaxUsages = dto.MaxUsages;

            await _db.SaveChangesAsync();
            return Ok(voucher);
        }

        // Chỉ Owner xóa voucher
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound(new { message = "Voucher không tồn tại." });

            _db.Vouchers.Remove(voucher);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        public class ApplyVoucherDto
        {
            public string Code { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; } = 0;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyVoucherDto dto)
        {
            var voucher = await _db.Vouchers.FirstOrDefaultAsync(v => v.Code == dto.Code);

            if (voucher == null) return NotFound(new { message = "Mã giảm giá không tồn tại." });

            if (voucher.ExpiryDate < DateTime.UtcNow) return BadRequest(new { message = "Mã giảm giá đã hết hạn." });

            if (voucher.MaxUsages > 0 && voucher.UsedCount >= voucher.MaxUsages) return BadRequest(new { message = "Mã giảm giá đã hết lượt sử dụng." });

            if (voucher.MinOrderValue > 0 && dto.TotalAmount < voucher.MinOrderValue) 
                return BadRequest(new { message = $"Đơn hàng tối thiểu phải từ {voucher.MinOrderValue:N0}đ để áp dụng voucher này." });

            return Ok(new {
                voucher.Id,
                voucher.Code,
                voucher.Type,
                voucher.Value
            });
        }
    }
}
