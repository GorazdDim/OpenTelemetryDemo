using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo.Shared;

var builder = WebApplication.CreateBuilder(args);

var agentHost = builder.Configuration.GetSection("Jaeger:AgentHost").Get<string>();
var agentPort = builder.Configuration.GetSection("Jaeger:AgentPort").Get<int>();
var jaegerEndpoint = new Uri("http://" + agentHost + ":" + agentPort);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder =>
        resourceBuilder.AddService(OpenTelemetryConfig.ServiceProxyName)
    )
    .WithTracing(tracing =>
    {
        tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddProcessor<CustomProcessor>()
        .AddOtlpExporter(otlpExp => otlpExp.Endpoint = jaegerEndpoint)
        .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter();
    });


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () => "Hello World")
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.Run();
