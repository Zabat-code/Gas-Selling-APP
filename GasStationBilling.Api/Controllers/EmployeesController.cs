using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Utils;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;
    public EmployeesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<EmployeeResponse>>> GetAll()
        => await _db.Employees
            .AsNoTracking()
            .Select(e => new EmployeeResponse(e.Id, e.Name, e.Username, e.IsAdmin))
            .ToListAsync();

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create(CreateEmployeeRequest request)
    {
        // This system doesn't use session tokens, so the admin check is done
        // by confirming that the EmployeeId sent by the frontend (saved
        // locally after login) really belongs to an administrator.
        var requester = await _db.Employees.FindAsync(request.RequesterId);
        if (requester is null || !requester.IsAdmin)
            return StatusCode(403, "Only an administrator can create users");

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required");

        if (await _db.Employees.AnyAsync(e => e.Username == request.Username))
            return BadRequest("That username already exists");

        var employee = new Employee
        {
            Name = request.Name,
            Username = request.Username,
            PasswordHash = PasswordHasher.Hash(request.Password),
            IsAdmin = request.IsAdmin
        };

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        return new EmployeeResponse(employee.Id, employee.Name, employee.Username, employee.IsAdmin);
    }
}
