using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Services;

public interface ISettingsService
{
    Task<Settings> GetAsync();
    Task<Settings> UpdateAsync(SettingsRequest request);
}