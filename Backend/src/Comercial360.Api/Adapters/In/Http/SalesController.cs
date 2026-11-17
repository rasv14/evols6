using Comercial360.Application;
using Comercial360.Application.Sales;
using Microsoft.AspNetCore.Mvc;

namespace Comercial360.Api.Adapters.In.Http;

[ApiController]
[Route("api/sales")]
public sealed class SalesController(ISalesService salesService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(SaleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SaleResponse>> Register(
        [FromBody] RegisterSaleRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await salesService.RegisterAsync(request, idempotencyKey ?? string.Empty, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.SaleId }, result);
        }
        catch (SaleValidationException exception)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Solicitud inválida", detail: exception.Message);
        }
        catch (SaleConflictException exception)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Conflicto de venta", detail: exception.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SaleResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var sale = await salesService.GetSaleAsync(id, cancellationToken);
        return sale is null ? NotFound() : Ok(sale);
    }
}
