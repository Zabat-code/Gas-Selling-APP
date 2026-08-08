using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Fix the port explicitly so it's always the same
// (avoids .NET picking a different port each time)
builder.WebHost.UseUrls("http://localhost:5000");

// Without a launchSettings.json, dotnet run starts in "Production" mode
// by default, which disables Swagger and detailed error pages.
// Force "Development" for local dev.
builder.Environment.EnvironmentName = "Development";

// SQLite database (the .db file is created automatically in the project folder)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Allow enums like PaymentMethod to travel as text ("Cash")
        // instead of a number (0), which is what the frontend sends.
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());

        // Product has a list of Purchases, and each Purchase points back to
        // its Product. Without this, converting to JSON enters an infinite
        // cycle (Product -> Purchases -> Product -> Purchases -> ...).
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Lets the React frontend (running on a different port) call this API
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

// Creates the database (and applies the seed data from AppDbContext.OnModelCreating)
// automatically on startup. EnsureCreated() works without pre-generated migrations,
// so the app is self-contained: just run it and the schema + default users exist.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");
app.UseAuthorization();
app.MapControllers();

app.Run();
