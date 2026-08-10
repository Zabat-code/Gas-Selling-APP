namespace GasStationBilling.Api.Models;

public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsAdmin { get; set; }

    // Allows a non-admin user to edit product prices (sale/purchase).
    // The tank capacity remains restricted to administrators.
    public bool CanModifyPrices { get; set; }

    public List<Sale> Sales { get; set; } = new();
}
