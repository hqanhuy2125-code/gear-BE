using System;
using System.Collections.Generic;

namespace GamingGearBackend.DTOs
{
    public class FlashSaleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public IEnumerable<int> ProductIds { get; set; } = new List<int>();
        public bool IsActive { get; set; } = true;
    }
}
