using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using GamingGearBackend.Models;
using System;

namespace GamingGearBackend.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPaySettings _settings;

        public VnPayService(IOptions<VnPaySettings> settings)
        {
            _settings = settings.Value;
        }

        public string CreatePaymentUrl(int orderId, decimal amount, HttpContext context)
        {
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _settings.TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString()); // vnpay requires * 100
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            
            // Real ip address
            vnpay.AddRequestData("vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {orderId}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", $"{orderId}_{DateTime.Now.Ticks}"); // Unique per transaction

            var paymentUrl = vnpay.CreateRequestUrl(_settings.BaseUrl, _settings.HashSecret);
            return paymentUrl;
        }

        public bool ValidateSignature(IQueryCollection query)
        {
            var vnpay = new VnPayLibrary();
            foreach (var key in query.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, query[key]);
                }
            }

            var inputHash = query["vnp_SecureHash"].ToString();
            return vnpay.ValidateSignature(inputHash, _settings.HashSecret);
        }
    }
}
