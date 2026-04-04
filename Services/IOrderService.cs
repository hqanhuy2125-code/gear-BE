using System.Threading.Tasks;

namespace GamingGearBackend.Services
{
    public interface IOrderService
    {
        Task<bool> ConfirmPaymentAsync(string orderCode, decimal amount, string referenceCode, string? gateway = null, string? rawData = null);
    }
}
