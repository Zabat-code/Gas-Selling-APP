using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Services;

public class SettingsService : ISettingsService
{
    private readonly AppDbContext _db;

    public SettingsService(AppDbContext db) => _db = db;

    public async Task<Settings> GetAsync()
    {
        var settings = await _db.Settings.FindAsync(1);
        return settings ?? new Settings();
    }

        public async Task<Settings> UpdateAsync(SettingsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.StationName))
            throw new InvalidOperationException("Name cannot be empty");

        if (request.TaxRate < 0)
            throw new InvalidOperationException("Tax rate cannot be negative");

        var settings = await _db.Settings.FindAsync(1);
        if (settings is null)
        {
            settings = new Settings
            {
                Id = 1,
                StationName = request.StationName,
                AutoPrintInvoice = request.AutoPrintInvoice,
                TaxRate = request.TaxRate
            };
            _db.Settings.Add(settings);
        }
        else
        {
            settings.StationName = request.StationName;
            settings.AutoPrintInvoice = request.AutoPrintInvoice;
            settings.TaxRate = request.TaxRate;
        }

        await _db.SaveChangesAsync();
        return settings;
    }
}