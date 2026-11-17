using Comercial360.Application;
using Comercial360.Application.Ports.Out;
using Comercial360.Domain;
using Microsoft.EntityFrameworkCore;

namespace Comercial360.Adapters.Persistence;

public sealed class EfSalesRepository(Comercial360DbContext db) : ISalesRepository
{
    public async Task<IReadOnlyCollection<CatalogCustomerResponse>> GetCustomersAsync(
        CancellationToken cancellationToken) =>
        await db.Customers.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new CatalogCustomerResponse(x.Id, x.Name, x.DocumentNumber))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<CatalogProductResponse>> GetProductsAsync(
        CancellationToken cancellationToken) =>
        await db.Products.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Description)
            .Select(x => new CatalogProductResponse(
                x.Id, x.Code, x.Description, x.UnitNetPrice, x.TaxRate, x.Stock))
            .ToListAsync(cancellationToken);

    public Task<Sale?> GetByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        db.Sales
            .Include(x => x.Customer)
            .Include(x => x.Lines)
            .Include(x => x.ElectronicDocument)
            .SingleOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);

    public Task<Customer?> GetActiveCustomerAsync(Guid customerId, CancellationToken cancellationToken) =>
        db.Customers.SingleOrDefaultAsync(
            x => x.Id == customerId && x.IsActive, cancellationToken);

    public async Task<IReadOnlyCollection<Product>> GetActiveProductsAsync(
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken) =>
        await db.Products
            .Where(x => productIds.Contains(x.Id) && x.IsActive)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

    public Task<Sale?> GetByIdAsync(Guid saleId, CancellationToken cancellationToken) =>
        db.Sales.AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Lines)
            .Include(x => x.ElectronicDocument)
            .SingleOrDefaultAsync(x => x.Id == saleId, cancellationToken);

    public Task<ElectronicDocument?> GetElectronicDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken) =>
        db.ElectronicDocuments.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == documentId, cancellationToken);

    public void AddSale(Sale sale) => db.Sales.Add(sale);

    public void AddStockMovement(StockMovement movement) => db.StockMovements.Add(movement);

    public void AddOutboxMessage(OutboxMessage message) => db.OutboxMessages.Add(message);
}
