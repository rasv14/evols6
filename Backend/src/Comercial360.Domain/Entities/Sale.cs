namespace Comercial360.Domain;

public sealed class Sale
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public string RequestFingerprint { get; set; } = string.Empty;
    public string Status { get; set; } = "Registered";
    public decimal Subtotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public List<SaleLine> Lines { get; set; } = [];
    public ElectronicDocument? ElectronicDocument { get; set; }
}
