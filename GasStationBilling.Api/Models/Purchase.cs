namespace GasStationBilling.Api.Models;

public class Purchase
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public DateTime DateTime { get; set; } = DateTime.Now;

    public decimal QuantityGallons { get; set; }

    public decimal UnitPurchasePrice { get; set; }

    public decimal TotalCost { get; set; }

    public string Supplier { get; set; } = string.Empty;

    public string InvoiceNumber { get; set; } = string.Empty;
}
