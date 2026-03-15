using GamingGearBackend.Data;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/blogs")]
    public class BlogsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BlogsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAll()
        {
            var blogs = await _db.Blogs
                .OrderByDescending(b => b.Id)
                .ToListAsync();
            return Ok(blogs);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Blog>> GetById(int id)
        {
            var blog = await _db.Blogs.FindAsync(id);
            if (blog == null) return NotFound();
            return Ok(blog);
        }

        [HttpPost]
        public async Task<ActionResult<Blog>> Create([FromBody] Blog blog)
        {
            blog.Id = 0;
            if (blog.CreatedAt == default) blog.CreatedAt = DateTime.UtcNow;

            _db.Blogs.Add(blog);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = blog.Id }, blog);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Blog update)
        {
            var blog = await _db.Blogs.FindAsync(id);
            if (blog == null) return NotFound();

            blog.Title = update.Title;
            blog.Content = update.Content;
            blog.ImageUrl = update.ImageUrl;
            blog.Author = update.Author;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _db.Blogs.FindAsync(id);
            if (blog == null) return NotFound();

            _db.Blogs.Remove(blog);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

