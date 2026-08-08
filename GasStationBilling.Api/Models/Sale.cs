namespace GasStationBilling.Api.Models;

public enum PaymentMethod
{
    Cash,
    Card,
    Transfer
}

public class Sale
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateTime DateTime { get; set; } = DateTime.Now;

    public decimal QuantityGallons { get; set; }

    public decimal UnitSalePrice { get; set; }

    public decimal Total { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    public string TicketNumber { get; set; } = string.Empty;
}
