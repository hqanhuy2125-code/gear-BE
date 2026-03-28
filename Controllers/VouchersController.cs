using GamingGearBackend.Data;
using GamingGearBackend.Models;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vouchers = await _db.Vouchers.OrderByDescending(v => v.CreatedAt).ToListAsync();
            return Ok(vouchers);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Voucher dto)
        {
            if (await _db.Vouchers.AnyAsync(v => v.Code == dto.Code))
            {
                return BadRequest(new { message = "Mã voucher đã tồn tại." });
            }

            dto.CreatedAt = DateTime.UtcNow;
            _db.Vouchers.Add(dto);
            await _db.SaveChangesAsync();

            return Ok(dto);
        }

        public class ApplyVoucherDto
        {
            public string Code { get; set; } = string.Empty;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyVoucherDto dto)
        {
            var voucher = await _db.Vouchers.FirstOrDefaultAsync(v => v.Code == dto.Code);
            
            if (voucher == null) return NotFound(new { message = "Mã giảm giá không tồn tại." });

            if (voucher.ExpiryDate < DateTime.UtcNow) return BadRequest(new { message = "Mã giảm giá đã hết hạn." });

            if (voucher.MaxUsages > 0 && voucher.UsedCount >= voucher.MaxUsages) return BadRequest(new { message = "Mã giảm giá đã hết lượt sử dụng." });

            return Ok(new { 
                voucher.Id, 
                voucher.Code, 
                voucher.Type, 
                voucher.Value 
            });
        }
    }
}
