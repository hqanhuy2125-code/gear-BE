using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/drivers")]
    public class DriversController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DriversController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetAll()
        {
            var drivers = await _db.Drivers
                .OrderByDescending(d => d.Id)
                .ToListAsync();
            return Ok(drivers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Driver>> GetById(int id)
        {
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null) return NotFound();
            return Ok(driver);
        }

        [HttpPost]
        public async Task<ActionResult<Driver>> Create([FromBody] Driver driver)
        {
            driver.Id = 0;
            _db.Drivers.Add(driver);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Driver update)
        {
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null) return NotFound();

            driver.Name = update.Name;
            driver.Description = update.Description;
            driver.DownloadUrl = update.DownloadUrl;
            driver.Brand = update.Brand;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var driver = await _db.Drivers.FindAsync(id);
            if (driver == null) return NotFound();

            _db.Drivers.Remove(driver);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

