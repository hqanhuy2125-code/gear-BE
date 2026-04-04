using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/system-config")]
    [Authorize(Policy = "OwnerOnly")]
    public class SystemConfigController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SystemConfigController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var configs = await _db.SystemConfigs.ToListAsync();
            return Ok(configs);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] UpdateConfigDto dto)
        {
            var config = await _db.SystemConfigs.FirstOrDefaultAsync(c => c.Key == key);
            if (config == null) return NotFound(new { message = $"Config key '{key}' không tồn tại." });

            config.Value = dto.Value;
            config.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(config);
        }
    }

    public class UpdateConfigDto
    {
        public string Value { get; set; } = string.Empty;
    }
}
