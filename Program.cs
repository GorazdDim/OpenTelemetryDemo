using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo;
using OpenTelemetryDemo.Repositories;
using OpenTelemetryDemo.Repositories.Interfaces;
using OpenTelemetryDemo.Services;
using OpenTelemetryDemo.Services.Interfaces;

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
builder.Services.AddTransient<IStudentService, StudentService>();
builder.Services.AddTransient<IStudentRepository, StudentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/GetAllStudent", async (IStudentService studentService) => await studentService.GetAllStudents());

app.MapGet("/GetStudentById/{id}", async (int id, IStudentService studentService) => await studentService.GetStudentById(id));

app.MapPost("/SaveStudent", async ([FromBody] Student student, IStudentService studentService) =>
{
    await studentService.SaveStudent(student);

    return Results.Created($"/save/{student.Id}", student);
});

app.MapPut("/UpdateStudents/{id}", async ([FromBody] Student studentinput, IStudentService studentService) =>
{
    await studentService.UpdateStudent(studentinput);
    return Results.NoContent();
});

app.MapDelete("/DeleteStudent/{id}", async (int id, IStudentService studentService) =>
{
    await studentService.DeleteStudent(id);
    return Results.NoContent();
});

app.Run();
