using Microsoft.EntityFrameworkCore;
using GasStationBilling.Api.Models;

namespace GasStationBilling.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Settings> Settings => Set<Settings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Store the PaymentMethod enum as text in the database (more readable than a number)
        modelBuilder.Entity<Sale>()
            .Property(s => s.PaymentMethod)
            .HasConversion<string>();

        // ----- Seed data -----
        // "user1" is the employee who dispenses fuel (no admin permissions).
        // "admin1" is the only one who can create new users.
        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                Name = "Main Employee",
                Username = "user1",
                // SHA256 hash of "123456". Change this as soon as you set up the system.
                PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",
                IsAdmin = false,
                CanModifyPrices = false
            },
            new Employee
            {
                Id = 2,
                Name = "Administrator",
                Username = "admin1",
                // SHA256 hash of "admin123". Change this as soon as you set up the system.
                PasswordHash = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9",
                IsAdmin = true,
                CanModifyPrices = true
            }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Premium", CurrentSalePrice = 0, CurrentPurchasePrice = 0, TankCapacityGallons = 5000 },
            new Product { Id = 2, Name = "Regular", CurrentSalePrice = 0, CurrentPurchasePrice = 0, TankCapacityGallons = 5000 },
            new Product { Id = 3, Name = "Diesel", CurrentSalePrice = 0, CurrentPurchasePrice = 0, TankCapacityGallons = 5000 }
        );

        modelBuilder.Entity<Settings>().HasData(
            new Settings { Id = 1, StationName = "Station", AutoPrintInvoice = false, TaxRate = 0m }
        );
    }
}
