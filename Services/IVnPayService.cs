using Microsoft.AspNetCore.Http;
using GamingGearBackend.Models;

namespace GamingGearBackend.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(int orderId, decimal amount, HttpContext context);
        bool ValidateSignature(IQueryCollection query);
    }
}
