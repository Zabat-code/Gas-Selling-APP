namespace GasStationBilling.Api.Models;

// Single-row table (Id is always 1) for general system settings.
public class Settings
{
    public int Id { get; set; } = 1;

    public string StationName { get; set; } = "Station";
}
