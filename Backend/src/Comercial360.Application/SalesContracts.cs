namespace Comercial360.Application;

public sealed record RegisterSaleItemRequest(Guid ProductId, decimal Quantity);

public sealed record RegisterSaleRequest(
    Guid CustomerId,
    string DocumentType,
    IReadOnlyCollection<RegisterSaleItemRequest> Items);

public sealed record SaleLineResponse(
    Guid ProductId,
    string Description,
    decimal Quantity,
    decimal UnitNetPrice,
    decimal TaxRate,
    decimal NetAmount,
    decimal TaxAmount,
    decimal Total);

public sealed record SaleResponse(
    Guid SaleId,
    Guid CustomerId,
    string CustomerName,
    string DocumentType,
    string SaleStatus,
    decimal Subtotal,
    decimal TaxTotal,
    decimal Total,
    DateTimeOffset CreatedAtUtc,
    Guid ElectronicDocumentId,
    string ElectronicDocumentStatus,
    IReadOnlyCollection<SaleLineResponse> Items);

public sealed record CatalogCustomerResponse(Guid Id, string Name, string DocumentNumber);
public sealed record CatalogProductResponse(
    Guid Id, string Code, string Description, decimal UnitNetPrice, decimal TaxRate, decimal Stock);
public sealed record ElectronicDocumentResponse(
    Guid Id, Guid SaleId, string DocumentType, string Status, string? ExternalReference, DateTimeOffset CreatedAtUtc);

public sealed class SaleValidationException(string message) : Exception(message);
public sealed class SaleConflictException(string message) : Exception(message);
