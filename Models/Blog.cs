using System;
using System.ComponentModel.DataAnnotations;

namespace GamingGearBackend.Models
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public string Author { get; set; } = "Admin";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
