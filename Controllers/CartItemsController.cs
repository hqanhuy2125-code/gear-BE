using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/cartitems")]
    public class CartItemsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CartItemsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetAll()
        {
            var items = await _db.CartItems
                .Include(ci => ci.Product)
                .OrderByDescending(ci => ci.Id)
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CartItem>> GetById(int id)
        {
            var item = await _db.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == id);

            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<CartItem>> Create([FromBody] CartItem item)
        {
            item.Id = 0;

            var firstUser = await _db.Users.OrderBy(u => u.Id).FirstOrDefaultAsync();
            if (firstUser == null) return BadRequest(new { message = "No users in database." });
            item.UserId = firstUser.Id;

            var firstProduct = await _db.Products.OrderBy(p => p.Id).FirstOrDefaultAsync();
            if (firstProduct == null) return BadRequest(new { message = "No products in database." });
            item.ProductId = firstProduct.Id;

            _db.CartItems.Add(item);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CartItem update)
        {
            var item = await _db.CartItems.FindAsync(id);
            if (item == null) return NotFound();

            item.Quantity = update.Quantity;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.CartItems.FindAsync(id);
            if (item == null) return NotFound();

            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

