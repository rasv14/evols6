namespace Comercial360.Domain;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public Guid ElectronicDocumentId { get; set; }
    public string EventType { get; set; } = "ElectronicDocumentRequested";
    public string Status { get; set; } = "Pending";
    public int Attempts { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
