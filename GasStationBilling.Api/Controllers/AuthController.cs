using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Utils;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    public AuthController(AppDbContext db) => _db = db;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Username == request.Username);
        if (employee is null) return Unauthorized("Incorrect username or password");

        if (PasswordHasher.Hash(request.Password) != employee.PasswordHash)
            return Unauthorized("Incorrect username or password");

        return new LoginResponse(employee.Id, employee.Name, employee.IsAdmin);
    }
}
