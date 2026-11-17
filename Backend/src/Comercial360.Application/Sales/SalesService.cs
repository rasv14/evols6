using Comercial360.Application.Ports.Out;
using Comercial360.Domain;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Comercial360.Application.Sales;

public sealed class SalesService(ISalesRepository salesRepository, IUnitOfWork unitOfWork) : ISalesService
{
    public Task<IReadOnlyCollection<CatalogCustomerResponse>> GetCustomersAsync(
        CancellationToken cancellationToken) =>
        salesRepository.GetCustomersAsync(cancellationToken);

    public Task<IReadOnlyCollection<CatalogProductResponse>> GetProductsAsync(
        CancellationToken cancellationToken) =>
        salesRepository.GetProductsAsync(cancellationToken);

    public async Task<SaleResponse> RegisterAsync(
        RegisterSaleRequest request,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        ValidateRequest(request, idempotencyKey);
        var requestFingerprint = CreateFingerprint(request);
        await using var transaction = await unitOfWork.BeginSerializableTransactionAsync(cancellationToken);

        var priorSale = await salesRepository.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
        if (priorSale is not null)
        {
            if (!CryptographicOperations.FixedTimeEquals(
                    Convert.FromHexString(priorSale.RequestFingerprint),
                    Convert.FromHexString(requestFingerprint)))
                throw new SaleConflictException("La clave de idempotencia ya fue utilizada con datos distintos.");

            await transaction.CommitAsync(cancellationToken);
            return ToResponse(priorSale);
        }

        var customer = await salesRepository.GetActiveCustomerAsync(request.CustomerId, cancellationToken)
            ?? throw new SaleValidationException("El cliente no existe o está inactivo.");

        var requestedQuantities = request.Items
            .GroupBy(x => x.ProductId)
            .ToDictionary(group => group.Key, group => group.Sum(x => x.Quantity));
        var products = await salesRepository.GetActiveProductsAsync(
            requestedQuantities.Keys.ToArray(), cancellationToken);
        if (products.Count != requestedQuantities.Count)
            throw new SaleValidationException("Uno o más productos no existen o están inactivos.");

        foreach (var product in products)
        {
            if (product.Stock < requestedQuantities[product.Id])
                throw new SaleConflictException(
                    $"Stock insuficiente para {product.Description}. Disponible: {product.Stock}.");
        }

        var now = DateTimeOffset.UtcNow;
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Customer = customer,
            DocumentType = request.DocumentType,
            IdempotencyKey = idempotencyKey,
            RequestFingerprint = requestFingerprint,
            Status = "Registered",
            CreatedAtUtc = now
        };

        foreach (var product in products)
        {
            var quantity = requestedQuantities[product.Id];
            var netAmount = decimal.Round(
                quantity * product.UnitNetPrice, 2, MidpointRounding.AwayFromZero);
            var taxAmount = decimal.Round(
                netAmount * product.TaxRate, 2, MidpointRounding.AwayFromZero);
            sale.Lines.Add(new SaleLine
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                DescriptionSnapshot = product.Description,
                Quantity = quantity,
                UnitNetPrice = product.UnitNetPrice,
                TaxRateSnapshot = product.TaxRate,
                NetAmount = netAmount,
                TaxAmount = taxAmount,
                Total = netAmount + taxAmount
            });
            product.Stock -= quantity;
            salesRepository.AddStockMovement(new StockMovement
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                SaleId = sale.Id,
                Quantity = -quantity,
                CreatedAtUtc = now
            });
        }

        sale.Subtotal = sale.Lines.Sum(x => x.NetAmount);
        sale.TaxTotal = sale.Lines.Sum(x => x.TaxAmount);
        sale.Total = sale.Subtotal + sale.TaxTotal;

        var document = new ElectronicDocument
        {
            Id = Guid.NewGuid(),
            SaleId = sale.Id,
            Sale = sale,
            DocumentType = request.DocumentType,
            Status = "Pending",
            CreatedAtUtc = now
        };
        sale.ElectronicDocument = document;
        salesRepository.AddSale(sale);
        salesRepository.AddOutboxMessage(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            ElectronicDocumentId = document.Id,
            EventType = "ElectronicDocumentRequested",
            Status = "Pending",
            CreatedAtUtc = now
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ToResponse(sale);
    }

    public async Task<SaleResponse?> GetSaleAsync(Guid saleId, CancellationToken cancellationToken)
    {
        var sale = await salesRepository.GetByIdAsync(saleId, cancellationToken);
        return sale is null ? null : ToResponse(sale);
    }

    public async Task<ElectronicDocumentResponse?> GetElectronicDocumentAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var document = await salesRepository.GetElectronicDocumentAsync(documentId, cancellationToken);
        return document is null
            ? null
            : new ElectronicDocumentResponse(
                document.Id, document.SaleId, document.DocumentType, document.Status,
                document.ExternalReference, document.CreatedAtUtc);
    }

    private static void ValidateRequest(RegisterSaleRequest request, string idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Length > 100)
            throw new SaleValidationException(
                "La clave de idempotencia es obligatoria y debe tener como máximo 100 caracteres.");
        if (request.CustomerId == Guid.Empty)
            throw new SaleValidationException("Seleccione un cliente válido.");
        if (request.DocumentType is not ("Invoice" or "BillOfSale"))
            throw new SaleValidationException("El tipo de comprobante debe ser Invoice o BillOfSale.");
        if (request.Items is null || request.Items.Count == 0 || request.Items.Count > 100)
            throw new SaleValidationException("La venta debe incluir entre 1 y 100 productos.");
        if (request.Items.Any(x => x.ProductId == Guid.Empty || x.Quantity <= 0 || x.Quantity > 100000))
            throw new SaleValidationException(
                "Cada producto debe ser válido y tener una cantidad mayor que cero.");
    }

    private static SaleResponse ToResponse(Sale sale)
    {
        var document = sale.ElectronicDocument
            ?? throw new InvalidOperationException("La venta no tiene un comprobante electrónico asociado.");
        return new SaleResponse(
            sale.Id, sale.CustomerId, sale.Customer?.Name ?? string.Empty, sale.DocumentType, sale.Status,
            sale.Subtotal, sale.TaxTotal, sale.Total, sale.CreatedAtUtc,
            document.Id, document.Status,
            sale.Lines.Select(x => new SaleLineResponse(
                x.ProductId, x.DescriptionSnapshot, x.Quantity, x.UnitNetPrice, x.TaxRateSnapshot,
                x.NetAmount, x.TaxAmount, x.Total)).ToArray());
    }

    private static string CreateFingerprint(RegisterSaleRequest request)
    {
        var normalizedItems = string.Join(
            "|",
            request.Items
                .GroupBy(x => x.ProductId)
                .OrderBy(group => group.Key)
                .Select(group =>
                    $"{group.Key:N}:{group.Sum(x => x.Quantity).ToString(CultureInfo.InvariantCulture)}"));
        var content = $"{request.CustomerId:N}|{request.DocumentType}|{normalizedItems}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
    }
}
