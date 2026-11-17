namespace Comercial360.Domain;

public sealed class StockMovement
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid SaleId { get; set; }
    public decimal Quantity { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
