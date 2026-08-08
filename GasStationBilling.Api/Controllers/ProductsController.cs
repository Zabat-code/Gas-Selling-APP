using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll()
        => await _db.Products.AsNoTracking().ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _db.Products.FindAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    // Updates sale price, purchase price and tank capacity in one call
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductRequest request)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        if (request.TankCapacityGallons <= 0) return BadRequest("Capacity must be greater than 0");

        product.CurrentSalePrice = request.CurrentSalePrice;
        product.CurrentPurchasePrice = request.CurrentPurchasePrice;
        product.TankCapacityGallons = request.TankCapacityGallons;
        await _db.SaveChangesAsync();

        return Ok(product);
    }
}
