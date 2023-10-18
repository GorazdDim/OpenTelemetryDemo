using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo;
using OpenTelemetryDemo.EF;
using OpenTelemetryDemo.EF.Entities;
using OpenTelemetryDemo.Repositories;
using OpenTelemetryDemo.Repositories.Interfaces;
using OpenTelemetryDemo.Services;
using OpenTelemetryDemo.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var agentHost = builder.Configuration.GetSection("Jaeger:AgentHost").Get<string>();
var agentPort = builder.Configuration.GetSection("Jaeger:AgentPort").Get<int>();
var jaeggerEndpoint = new Uri("http://" + agentHost + ":" + agentPort);

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
        .AddEntityFrameworkCoreInstrumentation()
        .AddProcessor<CustomProcessor>()
        .AddSource(CustomTraces.Default.Name)
        .AddOtlpExporter(otlpExp => otlpExp.Endpoint = jaeggerEndpoint)
        .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddProcessInstrumentation()
        .AddMeter(CustomMetrics.Default.Name)
        .AddView(CustomMetrics.UpdateStudentDelay.Name, CustomMetrics.UpdateStudentDelayView)
        .AddPrometheusExporter();
    });

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
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
    CustomMetrics.StudentsCreated.Add(1);
    await studentService.SaveStudent(student);

    return Results.Created($"/save/{student.Id}", student);
});

studentsApp.MapPut("/Update", async ([FromBody] Student student, IStudentService studentService) =>
{
    var random = new Random().Next(50, 100);
    // publish ping delay metrics
    CustomMetrics.UpdateStudentDelay.Record(random);
    await studentService.UpdateStudent(student);
    return Results.NoContent();
});

studentsApp.MapDelete("/Delete/{id}", async (int id, IStudentService studentService) =>
{
    await studentService.DeleteStudent(id);
    return Results.NoContent();
});

var professorsApp = app.MapGroup("/professors");

professorsApp.MapGet("/GetAll", async (HttpContext context, IProfessorService professorService) =>
{
    using (var activity = CustomTraces.Default.StartActivity("GetAllProfessorsEndpoint"))
    {
        activity?.SetTag("http.method", context.Request.Method);
        activity?.SetTag("http.url", context.Request.Path);
        activity?.SetTag("http.host", context.Request.Host.Value);
        activity?.SetTag("http.scheme", context.Request.Scheme);

        var random = new Random().Next(50, 100);
        await Task.Delay(random);
        activity?.SetTag("otl-demo.delay", random);
        return await professorService.GetAllProfessors();
    }
});

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
