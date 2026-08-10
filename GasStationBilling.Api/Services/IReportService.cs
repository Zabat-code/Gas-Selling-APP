using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Services;

public interface IReportService
{
    Task<SummaryReport> GetSummaryAsync(DateTime? from, DateTime? to);
}