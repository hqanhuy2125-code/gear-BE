using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingGearBackend.Models
{
    public class PaymentLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public string TransactionId { get; set; } = string.Empty;

        public string? Gateway { get; set; }

        public DateTime? TransactionDate { get; set; }

        public string? RawData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
