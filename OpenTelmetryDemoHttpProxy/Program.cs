using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo.Shared;

var builder = WebApplication.CreateBuilder(args);

var jaegerEndpoint = Configuration.GetJaegerEndpoint(builder.Configuration);
var serviceEndpoint = Configuration.GetServiceEndpoint(builder.Configuration);

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
builder.Services.AddHttpClient("otel-proxy-app", config => { config.BaseAddress = serviceEndpoint; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("", () => "Hello World")
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("students/GetAll", async (HttpContext context, IHttpClientFactory clientFactory) =>
{
    var httpClient = clientFactory.CreateClient("otel-proxy-app");
    var response = await httpClient.GetAsync("/students/GetAll");

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(content);
    }
    else
    {
        context.Response.StatusCode = (int)response.StatusCode;
    }
});

app.MapGet("professors/GetAll", async (HttpContext context, IHttpClientFactory clientFactory) =>
{
    var httpClient = clientFactory.CreateClient("otel-proxy-app");
    var response = await httpClient.GetAsync("/professors/GetAll");

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        context.Response.StatusCode = 200;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(content);
    }
    else
    {
        context.Response.StatusCode = (int)response.StatusCode;
    }
});

app.MapHealthChecks("/healthz/readiness");
app.MapHealthChecks("/healthz/liveness");

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.Run();
