using System.ComponentModel.DataAnnotations;

namespace GamingGearBackend.DTOs
{
    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class UpdateOrderDto
    {
        [Required]
        public string Status { get; set; } = "Pending";
    }
}

