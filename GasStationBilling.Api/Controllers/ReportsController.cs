using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReportsController(AppDbContext db) => _db = db;

    [HttpGet("summary")]
    public async Task<ActionResult<SummaryReport>> Summary(DateTime? from, DateTime? to)
    {
        var start = from ?? DateTime.Today;
        var end = to ?? DateTime.Now;

        var sales = await _db.Sales
            .Include(s => s.Product)
            .Where(s => s.DateTime >= start && s.DateTime <= end)
            .ToListAsync();

        var purchases = await _db.Purchases
            .Include(p => p.Product)
            .Where(p => p.DateTime >= start && p.DateTime <= end)
            .ToListAsync();

        var products = await _db.Products.AsNoTracking().ToListAsync();

        var byProduct = products.Select(p => new ProductSummary(
            p.Name,
            sales.Where(s => s.ProductId == p.Id).Sum(s => s.QuantityGallons),
            sales.Where(s => s.ProductId == p.Id).Sum(s => s.Total),
            purchases.Where(x => x.ProductId == p.Id).Sum(x => x.QuantityGallons),
            purchases.Where(x => x.ProductId == p.Id).Sum(x => x.TotalCost)
        )).ToList();

        var totalSales = sales.Sum(s => s.Total);
        var totalPurchases = purchases.Sum(p => p.TotalCost);

        return new SummaryReport(start, end, totalSales, totalPurchases, totalSales - totalPurchases, byProduct);
    }
}
