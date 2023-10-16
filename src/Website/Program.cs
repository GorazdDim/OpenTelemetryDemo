using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo;
using OpenTelemetryDemo.EF.Entities;
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
        .AddHttpClientInstrumentation()
        .AddProcessor<CustomProcessor>()
        .AddOtlpExporter(otlpExp => otlpExp.Endpoint = OpenTelemetryConfig.JaeggerEndpoint)
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

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddTransient<IStudentService, StudentService>();
builder.Services.AddTransient<IStudentRepository, StudentRepository>();
builder.Services.AddTransient<IProfessorService, ProfessorService>();
builder.Services.AddTransient<IProfessorRepository, ProfessorRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var studentsApp = app.MapGroup("/students");

studentsApp.MapGet("/GetAll", async (IStudentService studentService) => await studentService.GetAllStudents());

studentsApp.MapGet("/GetId/{id}", async (int id, IStudentService studentService) => await studentService.GetStudentById(id));

studentsApp.MapPost("/Save", async ([FromBody] Student student, IStudentService studentService) =>
{
    await studentService.SaveStudent(student);

    return Results.Created($"/save/{student.Id}", student);
});

studentsApp.MapPut("/Update/{id}", async ([FromBody] Student student, IStudentService studentService) =>
{
    await studentService.UpdateStudent(student);
    return Results.NoContent();
});

studentsApp.MapDelete("/Delete/{id}", async (int id, IStudentService studentService) =>
{
    await studentService.DeleteStudent(id);
    return Results.NoContent();
});

var professorsApp = app.MapGroup("/professors");

professorsApp.MapGet("/GetAll", async (IProfessorService professorService) => await professorService.GetAllProfessors());

professorsApp.MapGet("/GetId/{id}", async (int id, IProfessorService professorService) => await professorService.GetProfessorById(id));

professorsApp.MapPost("/Save", async ([FromBody] Professor professor, IProfessorService professorService) =>
{
    await professorService.SaveProfessor(professor);

    return Results.Created($"/save/{professor.Id}", professor);
});

professorsApp.MapPut("/Update/{id}", async ([FromBody] Professor professor, IProfessorService professorService) =>
{
    await professorService.UpdateProfessor(professor);
    return Results.NoContent();
});

professorsApp.MapDelete("/Delete/{id}", async (int id, IProfessorService professorService) =>
{
    await professorService.DeleteProfessor(id);
    return Results.NoContent();
});


app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.Run();
