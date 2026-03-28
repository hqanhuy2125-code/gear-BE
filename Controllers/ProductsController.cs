using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("fixed")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProductsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "page", "pageSize" })]
        public async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var totalItems = await _db.Products.CountAsync();
            var products = await _db.Products
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = products
            });
        }

        [HttpGet("search")]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "q", "category", "minPrice", "maxPrice", "sortBy", "page", "pageSize" })]
        public async Task<ActionResult> Search(
            [FromQuery] string? q,
            [FromQuery] string? category,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p => p.Name.Contains(q) || p.Description.Contains(q) || p.Category.Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "all")
            {
                query = query.Where(p => p.Category == category);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Sorting
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                "bestseller" => query.OrderByDescending(p => p.Stock), // Mocking bestseller using stock for now
                _ => query.OrderByDescending(p => p.Id)
            };

            var totalItems = await query.CountAsync();
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = products
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            product.Id = 0;
            if (product.CreatedAt == default) product.CreatedAt = DateTime.UtcNow;

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id:int}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> Update(int id, [FromBody] Product update)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = update.Name;
            product.Description = update.Description;
            product.Price = update.Price;
            product.ImageUrl = update.ImageUrl;
            product.Category = update.Category;
            product.Stock = update.Stock;
            product.IsPreOrder = update.IsPreOrder;
            product.PreOrderDate = update.PreOrderDate;
            product.IsOrderOnly = update.IsOrderOnly;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/stock")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int stock)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Stock = stock;
            await _db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPatch("{id:int}/toggle-preorder")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> TogglePreOrder(int id, [FromBody] bool isPreOrder)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsPreOrder = isPreOrder;
            if (isPreOrder && !product.PreOrderDate.HasValue)
            {
                product.PreOrderDate = DateTime.UtcNow.AddMonths(1);
            }
            await _db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPatch("{id:int}/toggle-orderonly")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> ToggleOrderOnly(int id, [FromBody] bool isOrderOnly)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsOrderOnly = isOrderOnly;
            await _db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

