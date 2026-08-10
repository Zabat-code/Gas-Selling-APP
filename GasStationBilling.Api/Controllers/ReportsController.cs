using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Services;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    [HttpGet("summary")]
    public async Task<ActionResult<SummaryReport>> Summary(DateTime? from, DateTime? to)
        => await _reports.GetSummaryAsync(from, to);
}
