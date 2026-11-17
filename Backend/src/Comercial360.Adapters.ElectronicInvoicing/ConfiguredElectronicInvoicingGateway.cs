using Comercial360.Application.Ports.Out;

namespace Comercial360.Adapters.ElectronicInvoicing;

public sealed class ConfiguredElectronicInvoicingGateway(
    IConfiguredElectronicTransport transport) : IElectronicInvoicingGateway
{
    public Task<ElectronicSubmissionResult> SubmitAsync(
        ElectronicDocumentPayload payload,
        CancellationToken cancellationToken) =>
        transport.SubmitAsync(payload, cancellationToken);
}

public interface IConfiguredElectronicTransport
{
    Task<ElectronicSubmissionResult> SubmitAsync(
        ElectronicDocumentPayload payload,
        CancellationToken cancellationToken);
}
