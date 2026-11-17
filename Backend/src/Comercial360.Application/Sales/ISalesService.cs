namespace Comercial360.Application.Sales;

public interface ISalesService
{
    Task<IReadOnlyCollection<CatalogCustomerResponse>> GetCustomersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<CatalogProductResponse>> GetProductsAsync(CancellationToken cancellationToken);
    Task<SaleResponse> RegisterAsync(
        RegisterSaleRequest request,
        string idempotencyKey,
        CancellationToken cancellationToken);
    Task<SaleResponse?> GetSaleAsync(Guid saleId, CancellationToken cancellationToken);
    Task<ElectronicDocumentResponse?> GetElectronicDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken);
}
