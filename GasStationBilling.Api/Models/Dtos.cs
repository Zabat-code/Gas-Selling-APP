using System.ComponentModel.DataAnnotations;

namespace GasStationBilling.Api.Models;

// Input DTOs are plain classes so [Range]/[Required] attributes validate on
// the bound properties and invalid values are rejected with 400 automatically.
public class LoginRequest
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public record LoginResponse(int EmployeeId, string Name, bool IsAdmin, bool CanModifyPrices, string Token);

public class CreateEmployeeRequest
{
    [Required, StringLength(50)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Username { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool CanModifyPrices { get; set; }
}

public record EmployeeResponse(int Id, string Name, string Username, bool IsAdmin, bool CanModifyPrices);

public class UpdateEmployeePermissionsRequest
{
    public bool CanModifyPrices { get; set; }
}

// Prices validated with [Range] so invalid/negative values return 400 from
// model binding, before they ever reach the business layer.
public class UpdateProductRequest
{
    [Range(0.0, 999999999.0)] public decimal CurrentSalePrice { get; set; }
    [Range(0.0, 999999999.0)] public decimal CurrentPurchasePrice { get; set; }
}

public class UpdateTankCapacityRequest
{
    [Range(0.0001, 999999999.0)] public decimal TankCapacityGallons { get; set; }
}

public class SettingsRequest
{
    [Required] public string StationName { get; set; } = string.Empty;
    public bool AutoPrintInvoice { get; set; }
    [Range(0.0, 1.0)] public decimal TaxRate { get; set; }
}

public class PurchaseRequest
{
    [Range(1, int.MaxValue)] public int ProductId { get; set; }
    [Range(0.0001, 999999999.0)] public decimal QuantityGallons { get; set; }
    [Range(0.0001, 999999999.0)] public decimal UnitPurchasePrice { get; set; }
    [Required] public string Supplier { get; set; } = string.Empty;
    [Required] public string InvoiceNumber { get; set; } = string.Empty;
}

public class SaleRequest
{
    [Range(1, int.MaxValue)] public int ProductId { get; set; }
    [Range(0.0001, 999999999.0)] public decimal QuantityGallons { get; set; }
    [Required] public PaymentMethod PaymentMethod { get; set; }
}

public record InventoryResponse(int ProductId, string Name, decimal TotalPurchased, decimal TotalSold, decimal CurrentStock, decimal TankCapacityGallons, decimal PercentFull);

public record SummaryReport(DateTime From, DateTime To, decimal TotalSales, decimal TotalPurchases, decimal GrossProfit, List<ProductSummary> ByProduct);

public record ProductSummary(string Product, decimal GallonsSold, decimal TotalSold, decimal GallonsPurchased, decimal TotalPurchased);