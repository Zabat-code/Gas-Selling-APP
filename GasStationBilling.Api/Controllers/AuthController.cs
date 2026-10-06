using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Models;
using GasStationBilling.Api.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GasStationBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // PUBLIC DEMO DEFAULT ONLY. Replace Jwt:Key before any real deployment.
    // Never use this value to protect real customer, employee, or payment data.
    private const string PublicDemoJwtKey = "PUBLIC-DEMO-ONLY-REPLACE-JWT-KEY-BEFORE-DEPLOYMENT-2026";

    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Username == request.Username);
        if (employee is null || !PasswordHasher.Verify(request.Password, employee.PasswordHash))
            return Unauthorized("Incorrect username or password");

        // Upgrade legacy (plain SHA256) hashes to PBKDF2 on first successful login.
        if (PasswordHasher.IsLegacyFormat(employee.PasswordHash))
        {
            employee.PasswordHash = PasswordHasher.Hash(request.Password);
            await _db.SaveChangesAsync();
        }

        var token = GenerateToken(employee);
        return new LoginResponse(employee.Id, employee.Name, employee.IsAdmin, employee.CanModifyPrices, token);
    }

    private string GenerateToken(Employee employee)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new(ClaimTypes.Name, employee.Name),
            new(ClaimTypes.Role, employee.IsAdmin ? "Admin" : "Staff"),
            new("canModifyPrices", (employee.IsAdmin || employee.CanModifyPrices).ToString().ToLowerInvariant())
        };

        var jwtKey = _config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey)) jwtKey = PublicDemoJwtKey;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "GasStationBilling",
            audience: _config["Jwt:Audience"] ?? "GasStationBillingClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
