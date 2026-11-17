namespace Comercial360.Domain;

public sealed class Product
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitNetPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Stock { get; set; }
    public bool IsActive { get; set; }
}
