using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _db;
    public SettingsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<Settings>> Get()
    {
        var settings = await _db.Settings.FindAsync(1);
        return settings ?? new Settings();
    }

    [HttpPut]
    public async Task<ActionResult<Settings>> Update(SettingsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.StationName))
            return BadRequest("Name cannot be empty");

        var settings = await _db.Settings.FindAsync(1);
        if (settings is null)
        {
            settings = new Settings { Id = 1, StationName = request.StationName };
            _db.Settings.Add(settings);
        }
        else
        {
            settings.StationName = request.StationName;
        }

        await _db.SaveChangesAsync();
        return settings;
    }
}
