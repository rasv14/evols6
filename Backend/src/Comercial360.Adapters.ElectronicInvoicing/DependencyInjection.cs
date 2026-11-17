using Comercial360.Application.Ports.Out;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Comercial360.Adapters.ElectronicInvoicing;

public static class DependencyInjection
{
    public static IServiceCollection AddElectronicInvoicingAdapter(this IServiceCollection services)
    {
        services.TryAddScoped<IConfiguredElectronicTransport, UnconfiguredElectronicTransport>();
        services.AddScoped<IElectronicInvoicingGateway, ConfiguredElectronicInvoicingGateway>();
        return services;
    }
}

internal sealed class UnconfiguredElectronicTransport : IConfiguredElectronicTransport
{
    public Task<ElectronicSubmissionResult> SubmitAsync(
        ElectronicDocumentPayload payload,
        CancellationToken cancellationToken) =>
        throw new InvalidOperationException(
            "No hay un transporte de emisión electrónica configurado para el proveedor elegido.");
}
