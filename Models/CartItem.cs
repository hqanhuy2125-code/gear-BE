using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GamingGearBackend.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public int Quantity { get; set; } = 1;

        [JsonIgnore]
        public User? User { get; set; }

        public Product? Product { get; set; }
    }
}
