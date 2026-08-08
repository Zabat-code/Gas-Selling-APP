using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _db;
    public InventoryController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<InventoryResponse>>> GetAll()
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

    [HttpGet("{productId}")]
    public async Task<ActionResult<InventoryResponse>> GetByProduct(int productId)
    {
        var p = await _db.Products.FindAsync(productId);
        if (p is null) return NotFound();

        var totalPurchased = (await _db.Purchases.Where(x => x.ProductId == productId).Select(x => x.QuantityGallons).ToListAsync()).Sum();
        var totalSold = (await _db.Sales.Where(x => x.ProductId == productId).Select(x => x.QuantityGallons).ToListAsync()).Sum();
        var stock = totalPurchased - totalSold;
        var percent = p.TankCapacityGallons > 0 ? (stock / p.TankCapacityGallons) * 100 : 0;

        return new InventoryResponse(p.Id, p.Name, totalPurchased, totalSold, stock, p.TankCapacityGallons, Math.Round(percent, 1));
    }
}
