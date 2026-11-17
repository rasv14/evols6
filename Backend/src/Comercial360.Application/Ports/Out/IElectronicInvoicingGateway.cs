namespace Comercial360.Application.Ports.Out;

public sealed record ElectronicDocumentPayload(
    Guid ElectronicDocumentId,
    string DocumentType,
    string ContentXml,
    string CorrelationId);

public sealed record ElectronicSubmissionResult(
    string ProviderStatus,
    bool IsAccepted,
    bool IsRejected,
    string? ExternalReference,
    string? ResponseXml,
    string? Message);

public interface IElectronicInvoicingGateway
{
    Task<ElectronicSubmissionResult> SubmitAsync(
        ElectronicDocumentPayload payload,
        CancellationToken cancellationToken);
}
