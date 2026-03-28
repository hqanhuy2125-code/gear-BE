using System.ComponentModel.DataAnnotations;

namespace GamingGearBackend.Models
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public decimal MinOrderValue { get; set; }

        public int DiscountPercent { get; set; }

        public decimal MaxDiscount { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
