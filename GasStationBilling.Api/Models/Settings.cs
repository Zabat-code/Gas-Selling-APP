namespace GasStationBilling.Api.Models;

// Single-row table (Id is always 1) for general system settings.
public class Settings
{
    public int Id { get; set; } = 1;

    public string StationName { get; set; } = "Station";

    // When true, the frontend opens the printable invoice automatically
    // after registering a sale (or purchase) instead of waiting for the user.
    public bool AutoPrintInvoice { get; set; }

    // Tax rate applied on sale invoices (e.g. 0.18 = 18% ITBIS in the DR).
    public decimal TaxRate { get; set; }
}
