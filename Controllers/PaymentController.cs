using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GamingGearBackend.DTOs;
using GamingGearBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GamingGearBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IOrderService orderService,
            IConfiguration configuration,
            ILogger<PaymentController> logger)
        {
            _orderService = orderService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("sepay-webhook")]
        public async Task<IActionResult> SepayWebhook([FromBody] SepayWebhookPayload payload)
        {
            try
            {
                // Authentication removed per request to ensure webhook processing (SePay verification).
                // We will rely on unique OrderCodes and TransferAmounts for validation in OrderService.

                // Only process "in" transfers
                if (payload.TransferType?.ToLower() != "in")
                {
                    return Ok(new { success = true, message = "Skipped non-in transfer" });
                }

                // Parse OrderCode from "code" field or "content"
                var orderCode = payload.Code;
                
                // If SePay didn't auto-parse "code" (e.g. because of SCYTOL prefix), 
                // we extract it manually from "content" using Regex.
                if (string.IsNullOrEmpty(orderCode) && !string.IsNullOrEmpty(payload.Content))
                {
                    // Pattern: ORD followed by optional hyphen and then 8-11 alphanumeric characters
                    // This handles both "ORD-XXXXXXXX" and "ORDXXXXXXXX"
                    var match = System.Text.RegularExpressions.Regex.Match(payload.Content, @"ORD-?[A-Z0-9]+", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        orderCode = match.Value.ToUpper();
                        
                        // Normalize: If we got "ORDXXXXXXXX" (11 chars), convert to "ORD-XXXXXXXX"
                        if (orderCode.Length == 11 && !orderCode.Contains("-"))
                        {
                            orderCode = "ORD-" + orderCode.Substring(3);
                        }
                        
                        _logger.LogInformation($"Extracted and normalized OrderCode '{orderCode}' from transaction content.");
                    }
                }

                if (string.IsNullOrEmpty(orderCode))
                {
                    _logger.LogWarning("SePay Webhook: Missing OrderCode in payload.");
                    return BadRequest("Missing OrderCode");
                }

                var rawData = JsonSerializer.Serialize(payload);
                var success = await _orderService.ConfirmPaymentAsync(
                    orderCode,
                    payload.TransferAmount,
                    payload.ReferenceCode ?? payload.Id.ToString(),
                    payload.Gateway,
                    rawData
                );

                if (success)
                {
                    return Ok(new { success = true, message = "Payment confirmed" });
                }

                return BadRequest("Order not found or amount mismatch");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SePay Webhook");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
