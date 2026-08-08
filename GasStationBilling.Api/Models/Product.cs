namespace GasStationBilling.Api.Models;

public class Product
{
    public int Id { get; set; }

    // "Premium", "Regular", "Diesel"
    public string Name { get; set; } = string.Empty;

    public decimal CurrentSalePrice { get; set; }

    public decimal CurrentPurchasePrice { get; set; }

    public decimal TankCapacityGallons { get; set; }

    public List<Purchase> Purchases { get; set; } = new();
    public List<Sale> Sales { get; set; } = new();
}
