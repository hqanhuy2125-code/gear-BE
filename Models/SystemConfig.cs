using System.ComponentModel.DataAnnotations;

namespace GamingGearBackend.Models
{
    public class SystemConfig
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
