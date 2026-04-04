using GamingGearBackend.Data;
using GamingGearBackend.DTOs;
using GamingGearBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
        {
            var users = await _db.Users
                .OrderByDescending(u => u.Id)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserResponseDto>> GetProfile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var u = await _db.Users.FindAsync(int.Parse(userIdStr));
            if (u == null) return NotFound();

            return Ok(new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            });
        }

        [HttpGet("{id:int}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return NotFound();

            return Ok(new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            });
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "OwnerOnly")]
        public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto dto)
        {
            if (await _db.Users.AnyAsync(x => x.Email == dto.Email))
                return Conflict(new { message = "Email already exists." });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "customer" : dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var result = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = dto.Name;
            user.Role = dto.Role;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "AdminOrOwner")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}/block")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> Block(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Role == "admin")
            {
                user.Role = "admin_blocked";
                await _db.SaveChangesAsync();
            }
            return NoContent();
        }

        [HttpPut("{id:int}/unblock")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "OwnerOnly")]
        public async Task<IActionResult> Unblock(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Role == "admin_blocked")
            {
                user.Role = "admin";
                await _db.SaveChangesAsync();
            }
            return NoContent();
        }
    }
}

