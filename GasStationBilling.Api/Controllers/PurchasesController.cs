using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly AppDbContext _db;
    public PurchasesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<Purchase>>> GetAll(DateTime? from, DateTime? to, int? productId)
    {
        var query = _db.Purchases.Include(p => p.Product).AsNoTracking().AsQueryable();

        if (from is not null) query = query.Where(p => p.DateTime >= from);
        if (to is not null) query = query.Where(p => p.DateTime <= to);
        if (productId is not null) query = query.Where(p => p.ProductId == productId);

        return await query.OrderByDescending(p => p.DateTime).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Purchase>> Register(PurchaseRequest request)
    {
        var product = await _db.Products.FindAsync(request.ProductId);
        if (product is null) return BadRequest("Product does not exist");

        if (request.QuantityGallons <= 0) return BadRequest("Quantity must be greater than 0");

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

        return CreatedAtAction(nameof(GetAll), new { }, purchase);
    }
}
