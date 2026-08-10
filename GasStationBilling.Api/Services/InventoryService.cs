using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GasStationBilling.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _db;

    public InventoryService(AppDbContext db) => _db = db;

    public async Task<List<InventoryResponse>> GetAllAsync()
    {
        var products = await _db.Products.AsNoTracking().ToListAsync();
        var result = new List<InventoryResponse>();

        foreach (var p in products)
        {
            var totalPurchased = (await _db.Purchases.Where(x => x.ProductId == p.Id).Select(x => x.QuantityGallons).ToListAsync()).Sum();
            var totalSold = (await _db.Sales.Where(x => x.ProductId == p.Id).Select(x => x.QuantityGallons).ToListAsync()).Sum();
            var stock = totalPurchased - totalSold;
            var percent = p.TankCapacityGallons > 0 ? (stock / p.TankCapacityGallons) * 100 : 0;

            result.Add(new InventoryResponse(p.Id, p.Name, totalPurchased, totalSold, stock, p.TankCapacityGallons, Math.Round(percent, 1)));
        }

        return result;
    }

    public async Task<InventoryResponse?> GetByProductAsync(int productId)
    {
        var p = await _db.Products.FindAsync(productId);
        if (p is null) return null;

        var totalPurchased = (await _db.Purchases.Where(x => x.ProductId == productId).Select(x => x.QuantityGallons).ToListAsync()).Sum();
        var totalSold = (await _db.Sales.Where(x => x.ProductId == productId).Select(x => x.QuantityGallons).ToListAsync()).Sum();
        var stock = totalPurchased - totalSold;
        var percent = p.TankCapacityGallons > 0 ? (stock / p.TankCapacityGallons) * 100 : 0;

        return new InventoryResponse(p.Id, p.Name, totalPurchased, totalSold, stock, p.TankCapacityGallons, Math.Round(percent, 1));
    }
}