using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> UpdatePricesAsync(int id, int requesterId, UpdateProductRequest request);
    Task<Product?> UpdateTankCapacityAsync(int id, int requesterId, UpdateTankCapacityRequest request);
}