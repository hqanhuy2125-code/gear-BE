using System;
using System.ComponentModel.DataAnnotations;

namespace GamingGearBackend.Models
{
    public class FlashSale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        /// <summary>Phần trăm giảm giá (0-100)</summary>
        public decimal DiscountPercent { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        /// <summary>Danh sách ProductId cách nhau bởi dấu phẩy</summary>
        public string ProductIds { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
