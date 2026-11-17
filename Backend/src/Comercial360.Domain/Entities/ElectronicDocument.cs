namespace Comercial360.Domain;

public sealed class ElectronicDocument
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Sale? Sale { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? ExternalReference { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
