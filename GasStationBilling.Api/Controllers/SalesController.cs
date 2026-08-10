using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Services;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISaleService _sales;
    public SalesController(ISaleService sales) => _sales = sales;

    [HttpGet]
    public async Task<ActionResult<List<Sale>>> GetAll(DateTime? from, DateTime? to, int? productId)
        => await _sales.GetAllAsync(from, to, productId);

    // Returns a single sale (used by the frontend to re-print a ticket).
    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetById(int id)
    {
        var sale = await _sales.GetByIdAsync(id);
        return sale is null ? NotFound() : Ok(sale);
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> Register(SaleRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var employeeId))
                return Unauthorized("Invalid token");

            var sale = await _sales.RegisterAsync(employeeId, request);
            return CreatedAtAction(nameof(GetAll), new { }, sale);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}