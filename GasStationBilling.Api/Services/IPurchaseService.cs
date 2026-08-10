using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Services;

public interface IPurchaseService
{
    Task<List<Purchase>> GetAllAsync(DateTime? from, DateTime? to, int? productId);
    Task<Purchase> RegisterAsync(PurchaseRequest request);
}