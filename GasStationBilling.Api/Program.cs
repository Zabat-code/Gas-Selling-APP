using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GasStationBilling.Api.Data;
using GasStationBilling.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000");
builder.Environment.EnvironmentName = "Development";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var jwtKey = builder.Configuration["Jwt:Key"] ?? "cambia-esta-clave-super-secreta-en-produccion-2026-abcdef123456";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GasStationBilling";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "GasStationBillingClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Database bootstrap: migration-aware with a legacy fallback. See comments below.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var conn = db.Database.GetDbConnection();
    conn.Open();
    bool IsEnvironment(string env) => builder.Environment.IsEnvironment(env);

    bool TableExists(string name)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@name";
        var p = cmd.CreateParameter();
        p.ParameterName = "@name";
        p.Value = name;
        cmd.Parameters.Add(p);
        return cmd.ExecuteScalar() is not null;
    }

    bool ColumnExists(string table, string column)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info({table})";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            if (string.Equals(reader["name"]?.ToString(), column, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    void AddColumn(string table, string column, string ddl)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {ddl}";
        cmd.ExecuteNonQuery();
    }

    if (IsEnvironment("Testing"))
    {
        // Integration tests use a throwaway DB: EnsureCreated is enough and
        // the tests do not depend on the migration files.
        db.Database.EnsureCreated();
    }
    else if (TableExists("Employees") && !TableExists("__EFMigrationsHistory"))
    {
        // Legacy database created before EF migrations existed.
        db.Database.EnsureCreated();
        if (!ColumnExists("Employees", "CanModifyPrices"))
            AddColumn("Employees", "CanModifyPrices", "INTEGER NOT NULL DEFAULT 0");
        if (!ColumnExists("Settings", "AutoPrintInvoice"))
            AddColumn("Settings", "AutoPrintInvoice", "INTEGER NOT NULL DEFAULT 0");
        if (!ColumnExists("Settings", "TaxRate"))
            AddColumn("Settings", "TaxRate", "REAL NOT NULL DEFAULT 0");
        using (var backfill = conn.CreateCommand())
        {
            backfill.CommandText = "UPDATE Employees SET CanModifyPrices = 1 WHERE Id = 2 AND IsAdmin = 1";
            backfill.ExecuteNonQuery();
        }
    }
    else if (TableExists("__EFMigrationsHistory"))
    {
        // Database already tracked by EF migrations: apply pending migrations.
        db.Database.Migrate();
    }
    else
    {
        // Fresh database with no migration history: create the schema directly
        // (EnsureCreated applies the seed data defined in AppDbContext).
        db.Database.EnsureCreated();
    }

        // Automatic backup of the production database (skipped in tests).
    if (!IsEnvironment("Testing") && TableExists("Employees"))
    {
        try
        {
            var backupsDir = Path.Combine(AppContext.BaseDirectory, "backups");
            Directory.CreateDirectory(backupsDir);
            var cs = builder.Configuration.GetConnectionString("Default") ?? "Data Source=bombagas.db";
            var source = cs.Split(';', StringSplitOptions.RemoveEmptyEntries)
                           .Select(p => p.Split('=', StringSplitOptions.RemoveEmptyEntries))
                           .FirstOrDefault(p => p.Length == 2 &&
                               p[0].Trim().Equals("Data Source", StringComparison.OrdinalIgnoreCase));
            var file = source?.Length == 2 ? source[1].Trim() : "bombagas.db";
            if (!Path.IsPathRooted(file))
                file = Path.Combine(AppContext.BaseDirectory, file);
            if (File.Exists(file))
            {
                var dest = Path.Combine(backupsDir,
                    $"bombagas_{DateTime.Now:yyyyMMddHHmmss}.db");
                File.Copy(file, dest);
                Console.WriteLine($"[backup] database copied to {dest}");
            }
        }
        catch (Exception ex)
        {
            app.Services.GetService<Microsoft.Extensions.Logging.ILogger<Program>>()?
                .LogWarning("Could not create DB backup: {Message}", ex.Message);
        }
    }

    conn.Close();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed so integration tests (WebApplicationFactory) can bootstrap the app.
public partial class Program { }
