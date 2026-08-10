using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GasStationBilling.Api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db) => _db = db;

    public async Task<List<Product>> GetAllAsync()
        => await _db.Products.AsNoTracking().ToListAsync();

    public async Task<Product?> GetByIdAsync(int id)
        => await _db.Products.FindAsync(id);

    public async Task<Product?> UpdatePricesAsync(int id, int requesterId, UpdateProductRequest request)
    {
        // Only a user with the "CanModifyPrices" permission (or an admin) may
        // change prices. The tank capacity remains restricted to administrators.
        var requester = await _db.Employees.FindAsync(requesterId);
        if (requester is null)
            throw new UnauthorizedAccessException("Requester does not exist");
        if (!requester.IsAdmin && !requester.CanModifyPrices)
            throw new UnauthorizedAccessException("You don't have permission to modify prices");

        return await UpdateCoreAsync(id, new Func<Product, Task>(
            async product =>
            {
                if (request.CurrentSalePrice < 0)
                    throw new InvalidOperationException("Sale price cannot be negative");

                if (request.CurrentPurchasePrice < 0)
                    throw new InvalidOperationException("Purchase price cannot be negative");

                product.CurrentSalePrice = request.CurrentSalePrice;
                product.CurrentPurchasePrice = request.CurrentPurchasePrice;
            }));
    }

    public async Task<Product?> UpdateTankCapacityAsync(int id, int requesterId, UpdateTankCapacityRequest request)
    {
        // Only an administrator can modify the tank/warehouse capacity.
        var requester = await _db.Employees.FindAsync(requesterId);
        if (requester is null || !requester.IsAdmin)
            throw new UnauthorizedAccessException("Only an administrator can modify the tank capacity");

        return await UpdateCoreAsync(id, new Func<Product, Task>(
            async product =>
            {
                if (request.TankCapacityGallons <= 0)
                    throw new InvalidOperationException("Capacity must be greater than 0");

                product.TankCapacityGallons = request.TankCapacityGallons;
            }));
    }

    private async Task<Product?> UpdateCoreAsync(int id, Func<Product, Task> apply)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return null;

        await apply(product);
        await _db.SaveChangesAsync();
        return product;
    }
}