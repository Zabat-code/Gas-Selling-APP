using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GasStationBilling.Api.Services;

public class SaleService : ISaleService
{
    private readonly AppDbContext _db;

    public SaleService(AppDbContext db) => _db = db;

    public async Task<List<Sale>> GetAllAsync(DateTime? from, DateTime? to, int? productId)
    {
        var query = _db.Sales
            .Include(s => s.Product)
            .Include(s => s.Employee)
            .AsNoTracking()
            .AsQueryable();

        if (from is not null) query = query.Where(s => s.DateTime >= from);
        if (to is not null) query = query.Where(s => s.DateTime <= to);
        if (productId is not null) query = query.Where(s => s.ProductId == productId);

        return await query.OrderByDescending(s => s.DateTime).ToListAsync();
    }

        public async Task<Sale?> GetByIdAsync(int id)
        => await _db.Sales
            .Include(s => s.Product)
            .Include(s => s.Employee)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Sale> RegisterAsync(int employeeId, SaleRequest request)
    {
        var product = await _db.Products.FindAsync(request.ProductId);
        if (product is null)
            throw new KeyNotFoundException("Product does not exist");

        var employee = await _db.Employees.FindAsync(employeeId);
        if (employee is null)
            throw new KeyNotFoundException("Employee does not exist");

        // --- Business rules ---
        if (request.QuantityGallons <= 0)
            throw new InvalidOperationException("Quantity must be greater than 0");

        if (product.CurrentSalePrice <= 0)
            throw new InvalidOperationException("Cannot sell: the sale price is not set (0)");

        // Check there's enough stock in the tank before selling
        var totalPurchased = (await _db.Purchases
            .Where(p => p.ProductId == request.ProductId)
            .Select(p => p.QuantityGallons).ToListAsync()).Sum();
        var totalSold = (await _db.Sales
            .Where(s => s.ProductId == request.ProductId)
            .Select(s => s.QuantityGallons).ToListAsync()).Sum();
        var stock = totalPurchased - totalSold;

        if (request.QuantityGallons > stock)
            throw new InvalidOperationException($"Insufficient stock. Available: {stock} gallons");

        var sale = new Sale
        {
            ProductId = request.ProductId,
            EmployeeId = employeeId,
            QuantityGallons = request.QuantityGallons,
            UnitSalePrice = product.CurrentSalePrice,
            Total = request.QuantityGallons * product.CurrentSalePrice,
            PaymentMethod = request.PaymentMethod,
            DateTime = DateTime.Now,
            TicketNumber = GenerateTicketNumber()
        };

        _db.Sales.Add(sale);
        await _db.SaveChangesAsync();

        return sale;
    }

    private static string GenerateTicketNumber()
    {
        // Unique even when several sales happen within the same second: the old
        // sequential approach (CountAsync + 1) let two requests read the same
        // count and generate the same ticket, causing a conflict. A short GUID
        // suffix removes that race entirely.
        var stamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return $"TCK-{stamp}-{suffix}";
    }
}