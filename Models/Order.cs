using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GamingGearBackend.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }
        
        [Required]
        public string Status { get; set; } = "Pending";

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "COD";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
