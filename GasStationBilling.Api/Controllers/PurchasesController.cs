using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Services;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchases;
    public PurchasesController(IPurchaseService purchases) => _purchases = purchases;

    [HttpGet]
    public async Task<ActionResult<List<Purchase>>> GetAll(DateTime? from, DateTime? to, int? productId)
        => await _purchases.GetAllAsync(from, to, productId);

    [HttpPost]
    public async Task<ActionResult<Purchase>> Register(PurchaseRequest request)
    {
        try
        {
            var purchase = await _purchases.RegisterAsync(request);
            return CreatedAtAction(nameof(GetAll), new { }, purchase);
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
