using GamingGearBackend.Data;
using GamingGearBackend.Models;
using GamingGearBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/flash-sales")]
    public class FlashSalesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public FlashSalesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _db.FlashSales
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            return Ok(sales);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _db.FlashSales.FindAsync(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpPost]
        [Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> Create([FromBody] FlashSaleDto dto)
        {
            // Validation
            if (dto.StartTime < DateTime.UtcNow.AddMinutes(-5)) 
                return BadRequest(new { message = "Thời gian bắt đầu không được trong quá khứ." });
            if (dto.StartTime >= dto.EndTime)
                return BadRequest(new { message = "Thời gian bắt đầu phải trước thời gian kết thúc." });
            if (dto.DiscountPercent < 1 || dto.DiscountPercent > 100)
                return BadRequest(new { message = "Phần trăm giảm giá phải từ 1 đến 100." });

            var flashSale = new FlashSale
            {
                Name = dto.Name,
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                ProductIds = dto.ProductIds != null ? string.Join(",", dto.ProductIds) : "",
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _db.FlashSales.Add(flashSale);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = flashSale.Id }, flashSale);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] FlashSaleDto dto)
        {
            var sale = await _db.FlashSales.FindAsync(id);
            if (sale == null) return NotFound();

            // Validation
            if (dto.StartTime >= dto.EndTime)
                return BadRequest(new { message = "Thời gian bắt đầu phải trước thời gian kết thúc." });
            if (dto.DiscountPercent < 1 || dto.DiscountPercent > 100)
                return BadRequest(new { message = "Phần trăm giảm giá phải từ 1 đến 100." });

            sale.Name = dto.Name;
            sale.Description = dto.Description;
            sale.DiscountPercent = dto.DiscountPercent;
            sale.StartTime = dto.StartTime;
            sale.EndTime = dto.EndTime;
            sale.ProductIds = dto.ProductIds != null ? string.Join(",", dto.ProductIds) : "";
            sale.IsActive = dto.IsActive;

            await _db.SaveChangesAsync();
            return Ok(sale);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _db.FlashSales.FindAsync(id);
            if (sale == null) return NotFound();

            _db.FlashSales.Remove(sale);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/toggle")]
        [Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> Toggle(int id)
        {
            var sale = await _db.FlashSales.FindAsync(id);
            if (sale == null) return NotFound();

            sale.IsActive = !sale.IsActive;
            await _db.SaveChangesAsync();
            return Ok(sale);
        }
    }
}
