using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GamingGearBackend.DTOs
{
    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string ProductName { get; set; } = "Unknown Product";
        public decimal Price { get; set; } = 0;
        public string ImageUrl { get; set; } = "";
    }

    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new();

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "COD";
        public string? VoucherCode { get; set; }
    }

    public class UpdateOrderDto
    {
        [Required]
        public string Status { get; set; } = "Pending";
    }
}

