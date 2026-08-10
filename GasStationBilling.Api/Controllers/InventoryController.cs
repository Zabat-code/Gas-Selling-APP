using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Services;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventory;
    public InventoryController(IInventoryService inventory) => _inventory = inventory;

    [HttpGet]
    public async Task<ActionResult<List<InventoryResponse>>> GetAll()
        => await _inventory.GetAllAsync();

    [HttpGet("{productId}")]
    public async Task<ActionResult<InventoryResponse>> GetByProduct(int productId)
    {
        var result = await _inventory.GetByProductAsync(productId);
        return result is null ? NotFound() : Ok(result);
    }
}
