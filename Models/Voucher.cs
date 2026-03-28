using System;
using System.ComponentModel.DataAnnotations;

namespace GamingGearBackend.Models
{
    public class Voucher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = "percent"; // "percent" or "fixed"

        [Required]
        public decimal Value { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int MaxUsages { get; set; }

        public int UsedCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
