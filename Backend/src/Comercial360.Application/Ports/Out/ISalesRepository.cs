using Comercial360.Domain;

namespace Comercial360.Application.Ports.Out;

public interface ISalesRepository
{
    Task<IReadOnlyCollection<CatalogCustomerResponse>> GetCustomersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<CatalogProductResponse>> GetProductsAsync(CancellationToken cancellationToken);
    Task<Sale?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken);
    Task<Customer?> GetActiveCustomerAsync(Guid customerId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Product>> GetActiveProductsAsync(
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken);
    Task<Sale?> GetByIdAsync(Guid saleId, CancellationToken cancellationToken);
    Task<ElectronicDocument?> GetElectronicDocumentAsync(Guid documentId, CancellationToken cancellationToken);
    void AddSale(Sale sale);
    void AddStockMovement(StockMovement movement);
    void AddOutboxMessage(OutboxMessage message);
}
