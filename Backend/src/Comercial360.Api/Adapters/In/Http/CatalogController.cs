using Comercial360.Application;
using Comercial360.Application.Sales;
using Microsoft.AspNetCore.Mvc;

namespace Comercial360.Api.Adapters.In.Http;

[ApiController]
[Route("api/catalog")]
public sealed class CatalogController(ISalesService salesService) : ControllerBase
{
    [HttpGet("customers")]
    public async Task<ActionResult<IReadOnlyCollection<CatalogCustomerResponse>>> GetCustomers(CancellationToken cancellationToken) =>
        Ok(await salesService.GetCustomersAsync(cancellationToken));

    [HttpGet("products")]
    public async Task<ActionResult<IReadOnlyCollection<CatalogProductResponse>>> GetProducts(CancellationToken cancellationToken) =>
        Ok(await salesService.GetProductsAsync(cancellationToken));
}
