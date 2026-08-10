using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Services;

public interface IInventoryService
{
    Task<List<InventoryResponse>> GetAllAsync();
    Task<InventoryResponse?> GetByProductAsync(int productId);
}