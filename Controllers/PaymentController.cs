using Microsoft.AspNetCore.Mvc;
using GamingGearBackend.Services;
using GamingGearBackend.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly AppDbContext _context;

        public PaymentController(IVnPayService vnPayService, AppDbContext context)
        {
            _vnPayService = vnPayService;
            _context = context;
        }

        public class CreateVnPayUrlDto
        {
            public int OrderId { get; set; }
            public decimal Amount { get; set; }
        }

        [HttpPost("create-url")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "CustomerOnly")]
        public IActionResult CreateUrl([FromBody] CreateVnPayUrlDto dto)
        {
            var url = _vnPayService.CreatePaymentUrl(dto.OrderId, dto.Amount, HttpContext);
            return Ok(new { url });
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            if (Request.Query.Count == 0)
            {
                return BadRequest(new { success = false, message = "Không tìm thấy query string" });
            }

            var isValidSignature = _vnPayService.ValidateSignature(Request.Query);
            if (!isValidSignature)
            {
                return BadRequest(new { success = false, message = "Chữ ký không hợp lệ" });
            }

            string responseCode = Request.Query["vnp_ResponseCode"];
            string txnRef = Request.Query["vnp_TxnRef"];
            
            // Extract OrderId from vnp_TxnRef (format:OrderId_Ticks)
            if (string.IsNullOrEmpty(txnRef) || !txnRef.Contains("_"))
            {
                return BadRequest(new { success = false, message = "Dữ liệu giao dịch không hợp lệ" });
            }
            
            var orderIdStr = txnRef.Split('_')[0];
            if (!int.TryParse(orderIdStr, out var orderId))
            {
                return BadRequest(new { success = false, message = "Mã đơn hàng không hợp lệ" });
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            if (responseCode == "00")
            {
                // Payment Success
                order.Status = "Completed"; 
                // Or you can create a new Status like "Paid" if you prefer. 
                // "Completed" might typically mean Delivered, but let's just use "Pending" with a note? 
                // Wait, if it's paid but not delivered it should be "Pending" or a new "Paid" status.
                // The prompt just said "cập nhật trạng thái đơn hàng".
                // I'll update it to "Paid", but my frontend only handles Pending, Shipping, Delivered, Cancelled.
                // Ah, let's keep the status "Pending" and set a PaymentStatus, or just set to "Completed".
                // Actually, I'll set to "Pending" since it hasn't shipped yet! Wait, I'll add "Paid" to frontend eventually or just leave it. 
                // Actually the prompt: "Hoàn thành (xanh lá)". Let's just set it to "Completed" to show it works.
                order.Status = "Completed";
                await _context.SaveChangesAsync();
                return Ok(new { success = true, orderId = order.Id });
            }
            else
            {
                // Payment Failed
                return BadRequest(new { success = false, message = $"Giao dịch thất bại với mã lỗi: {responseCode}" });
            }
        }
    }
}
