using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GasStationBilling.Api.Services;

public class PurchaseService : IPurchaseService
{
    private readonly AppDbContext _db;

    public PurchaseService(AppDbContext db) => _db = db;

    public async Task<List<Purchase>> GetAllAsync(DateTime? from, DateTime? to, int? productId)
    {
        var query = _db.Purchases
            .Include(p => p.Product)
            .AsNoTracking()
            .AsQueryable();

        if (from is not null) query = query.Where(p => p.DateTime >= from);
        if (to is not null) query = query.Where(p => p.DateTime <= to);
        if (productId is not null) query = query.Where(p => p.ProductId == productId);

        return await query.OrderByDescending(p => p.DateTime).ToListAsync();
    }

    public async Task<Purchase> RegisterAsync(PurchaseRequest request)
    {
        var product = await _db.Products.FindAsync(request.ProductId);
        if (product is null)
            throw new KeyNotFoundException("Product does not exist");

        // --- Business rules ---
        if (request.QuantityGallons <= 0)
            throw new InvalidOperationException("Quantity must be greater than 0");

        if (request.UnitPurchasePrice < 0)
            throw new InvalidOperationException("Purchase price cannot be negative");

        var purchase = new Purchase
        {
            ProductId = request.ProductId,
            QuantityGallons = request.QuantityGallons,
            UnitPurchasePrice = request.UnitPurchasePrice,
            TotalCost = request.QuantityGallons * request.UnitPurchasePrice,
            Supplier = request.Supplier,
            InvoiceNumber = request.InvoiceNumber,
            DateTime = DateTime.Now
        };

        // Buying more fuel updates the product's current purchase price
        product.CurrentPurchasePrice = request.UnitPurchasePrice;

        _db.Purchases.Add(purchase);
        await _db.SaveChangesAsync();

        return purchase;
    }
}