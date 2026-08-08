using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _db;
    public SalesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<Sale>>> GetAll(DateTime? from, DateTime? to, int? productId)
    {
        var query = _db.Sales.Include(s => s.Product).Include(s => s.Employee).AsNoTracking().AsQueryable();

        if (from is not null) query = query.Where(s => s.DateTime >= from);
        if (to is not null) query = query.Where(s => s.DateTime <= to);
        if (productId is not null) query = query.Where(s => s.ProductId == productId);

        return await query.OrderByDescending(s => s.DateTime).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> Register(SaleRequest request)
    {
        var product = await _db.Products.FindAsync(request.ProductId);
        if (product is null) return BadRequest("Product does not exist");

        var employee = await _db.Employees.FindAsync(request.EmployeeId);
        if (employee is null) return BadRequest("Employee does not exist");

        if (request.QuantityGallons <= 0) return BadRequest("Quantity must be greater than 0");

        // Check there's enough stock in the tank before selling
        var totalPurchased = (await _db.Purchases.Where(p => p.ProductId == request.ProductId).Select(p => p.QuantityGallons).ToListAsync()).Sum();
        var totalSold = (await _db.Sales.Where(s => s.ProductId == request.ProductId).Select(s => s.QuantityGallons).ToListAsync()).Sum();
        var stock = totalPurchased - totalSold;

        if (request.QuantityGallons > stock)
            return BadRequest($"Insufficient stock. Available: {stock} gallons");

        var sale = new Sale
        {
            ProductId = request.ProductId,
            EmployeeId = request.EmployeeId,
            QuantityGallons = request.QuantityGallons,
            UnitSalePrice = product.CurrentSalePrice,
            Total = request.QuantityGallons * product.CurrentSalePrice,
            PaymentMethod = request.PaymentMethod,
            DateTime = DateTime.Now,
            TicketNumber = await GenerateTicketNumber()
        };

        _db.Sales.Add(sale);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { }, sale);
    }

    private async Task<string> GenerateTicketNumber()
    {
        var count = await _db.Sales.CountAsync();
        return $"TCK-{DateTime.Now:yyyyMMdd}-{(count + 1):D5}";
    }
}
