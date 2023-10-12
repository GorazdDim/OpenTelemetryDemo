using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.AddOpenTelemetry(otl =>
    {
        otl.SetResourceBuilder(
            ResourceBuilder
            .CreateDefault()
            .AddService(OpenTelemetryConfig.ServiceName))
        .AddOtlpExporter(otlpExp => otlpExp.Endpoint = OpenTelemetryConfig.JaeggerEndpoint)
        .AddConsoleExporter();
    });
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder =>
        resourceBuilder.AddService(OpenTelemetryConfig.ServiceName)
    )
    .WithTracing(tracing =>
    {
        tracing
        .AddAspNetCoreInstrumentation()
        //.AddSqlClientInstrumentation()
        //.AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter(otlpExp => otlpExp.Endpoint = OpenTelemetryConfig.JaeggerEndpoint)
        .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
        //.AddAspNetCoreInstrumentation()
        //.AddHttpClientInstrumentation()
        //.AddMeter(OpenTelemetryConfig.Meter.Name)
        //.AddRuntimeInstrumentation()
        .AddOtlpExporter(otlpExp => otlpExp.Endpoint = OpenTelemetryConfig.JaeggerEndpoint)
        .AddConsoleExporter();
    });

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/GetAllStudent", async (ApplicationDbContext db) =>
    await db.Students.ToListAsync());

app.MapPost("/SaveStudent", async ([FromBody]Student student, ApplicationDbContext db) =>
{
    db.Students.Add(student);
    await db.SaveChangesAsync();

    return Results.Created($"/save/{student.Id}", student);
});

app.MapPut("/UpdateStudents/{id}", async (int id, [FromBody] Student studentinput, ApplicationDbContext db) =>
{
    var student = await db.Students.FindAsync(id);

    if (student is null) return Results.NotFound();

    student.Name = studentinput.Name;
    student.Phone = studentinput.Phone;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
