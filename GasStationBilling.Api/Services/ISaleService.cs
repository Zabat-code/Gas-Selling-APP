using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Services;

public interface ISaleService
{
    Task<List<Sale>> GetAllAsync(DateTime? from, DateTime? to, int? productId);
    Task<Sale?> GetByIdAsync(int id);
    Task<Sale> RegisterAsync(int employeeId, SaleRequest request);
}