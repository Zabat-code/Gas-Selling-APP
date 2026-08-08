namespace GasStationBilling.Api.Models;

public record LoginRequest(string Username, string Password);
public record LoginResponse(int EmployeeId, string Name, bool IsAdmin);

public record CreateEmployeeRequest(int RequesterId, string Name, string Username, string Password, bool IsAdmin);
public record EmployeeResponse(int Id, string Name, string Username, bool IsAdmin);

public record UpdateProductRequest(decimal CurrentSalePrice, decimal CurrentPurchasePrice, decimal TankCapacityGallons);

public record SettingsRequest(string StationName);

public record PurchaseRequest(int ProductId, decimal QuantityGallons, decimal UnitPurchasePrice, string Supplier, string InvoiceNumber);

public record SaleRequest(int ProductId, int EmployeeId, decimal QuantityGallons, PaymentMethod PaymentMethod);

public record InventoryResponse(int ProductId, string Name, decimal TotalPurchased, decimal TotalSold, decimal CurrentStock, decimal TankCapacityGallons, decimal PercentFull);

public record SummaryReport(DateTime From, DateTime To, decimal TotalSales, decimal TotalPurchases, decimal GrossProfit, List<ProductSummary> ByProduct);

public record ProductSummary(string Product, decimal GallonsSold, decimal TotalSold, decimal GallonsPurchased, decimal TotalPurchased);
