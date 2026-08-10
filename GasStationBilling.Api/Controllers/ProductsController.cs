using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Services;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;
    public ProductsController(IProductService products) => _products = products;

    // The caller's id is always taken from the validated JWT (NameIdentifier),
    // never from the request body, so it cannot be spoofed.
    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll()
        => await _products.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _products.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    // Updates sale price and purchase price. Requires the "CanModifyPrices"
    // permission (granted by an admin) or an administrator account.
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePrices(int id, UpdateProductRequest request)
    {
        try
        {
            var product = await _products.UpdatePricesAsync(id, CurrentUserId(), request);
            if (product is null) return NotFound();
            return Ok(product);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Updates tank/warehouse capacity (administrator only).
    [HttpPut("{id}/tank-capacity")]
    public async Task<IActionResult> UpdateTankCapacity(int id, UpdateTankCapacityRequest request)
    {
        try
        {
            var product = await _products.UpdateTankCapacityAsync(id, CurrentUserId(), request);
            if (product is null) return NotFound();
            return Ok(product);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
