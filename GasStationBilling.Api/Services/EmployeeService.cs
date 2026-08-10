using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace GasStationBilling.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;

    public EmployeeService(AppDbContext db) => _db = db;

    public async Task<List<EmployeeResponse>> GetAllAsync()
        => await _db.Employees
            .AsNoTracking()
            .Select(e => new EmployeeResponse(e.Id, e.Name, e.Username, e.IsAdmin, e.CanModifyPrices))
            .ToListAsync();

    public async Task<EmployeeResponse> CreateAsync(int requesterId, CreateEmployeeRequest request)
    {
        // The requester id comes from the JWT, so it cannot be forged.
        var requester = await _db.Employees.FindAsync(requesterId);
        if (requester is null || !requester.IsAdmin)
            throw new UnauthorizedAccessException("Only an administrator can create users");

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Username and password are required");

        if (await _db.Employees.AnyAsync(e => e.Username == request.Username))
            throw new InvalidOperationException("That username already exists");

        var employee = new Employee
        {
            Name = request.Name,
            Username = request.Username,
            PasswordHash = PasswordHasher.Hash(request.Password),
            IsAdmin = request.IsAdmin,
            CanModifyPrices = !request.IsAdmin && request.CanModifyPrices
        };

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        return new EmployeeResponse(employee.Id, employee.Name, employee.Username, employee.IsAdmin, employee.CanModifyPrices);
    }

    public async Task<EmployeeResponse?> UpdatePermissionsAsync(int id, int requesterId, UpdateEmployeePermissionsRequest request)
    {
        // Only an administrator can grant or revoke the price-editing permission.
        var requester = await _db.Employees.FindAsync(requesterId);
        if (requester is null || !requester.IsAdmin)
            throw new UnauthorizedAccessException("Only an administrator can change user permissions");

        var employee = await _db.Employees.FindAsync(id);
        if (employee is null) return null;

        // Admins always have the permission; it can only be toggled for non-admins.
        employee.CanModifyPrices = !employee.IsAdmin && request.CanModifyPrices;
        await _db.SaveChangesAsync();

        return new EmployeeResponse(employee.Id, employee.Name, employee.Username, employee.IsAdmin, employee.CanModifyPrices);
    }
}