using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Services;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employees;
    public EmployeesController(IEmployeeService employees) => _employees = employees;

    // The caller's id is taken from the validated JWT (NameIdentifier).
    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponse>>> GetAll()
        => await _employees.GetAllAsync();

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create(CreateEmployeeRequest request)
    {
        try
        {
                        var employee = await _employees.CreateAsync(CurrentUserId(), request);
            return employee;
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

    // Grants or revokes the price-editing permission for a user (admin only).
    [HttpPut("{id}/permissions")]
    public async Task<IActionResult> UpdatePermissions(int id, UpdateEmployeePermissionsRequest request)
    {
        try
        {
                        var result = await _employees.UpdatePermissionsAsync(id, CurrentUserId(), request);
            if (result is null) return NotFound();
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }
}
