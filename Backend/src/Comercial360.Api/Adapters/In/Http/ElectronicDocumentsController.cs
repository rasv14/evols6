using Comercial360.Application;
using Comercial360.Application.Sales;
using Microsoft.AspNetCore.Mvc;

namespace Comercial360.Api.Adapters.In.Http;

[ApiController]
[Route("api/electronic-documents")]
public sealed class ElectronicDocumentsController(ISalesService salesService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ElectronicDocumentResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var document = await salesService.GetElectronicDocumentAsync(id, cancellationToken);
        return document is null ? NotFound() : Ok(document);
    }
}
