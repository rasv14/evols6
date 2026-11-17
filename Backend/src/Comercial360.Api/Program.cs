using Comercial360.Adapters.ElectronicInvoicing;
using Comercial360.Adapters.Persistence;
using Comercial360.Application.Sales;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Comercial360 API",
        Version = "v1",
        Description = "API para catálogo, registro de ventas y consulta de comprobantes electrónicos."
    });
});
builder.Services.AddProblemDetails();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddElectronicInvoicingAdapter();
builder.Services.AddScoped<ISalesService, SalesService>();

var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Comercial360 API v1"));
}
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();

public partial class Program;
