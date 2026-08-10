using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Services;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _settings;
    public SettingsController(ISettingsService settings) => _settings = settings;

    [HttpGet]
    public async Task<ActionResult<Settings>> Get()
        => await _settings.GetAsync();

    [HttpPut]
    public async Task<ActionResult<Settings>> Update(SettingsRequest request)
    {
        try
        {
            return await _settings.UpdateAsync(request);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
