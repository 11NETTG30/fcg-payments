using API.Configurations;
using Application.DependencyInjection;
using DotNetEnv;
using Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddDocumentation();
builder.Services.AddDb(builder.Configuration);
builder.Services.AddDI(builder.Configuration);
builder.Services.AddTelemetry(builder);
builder.Services.AddLogsTelemetry(builder);

builder.Services
    .AddApplication()
    .AddFluentValidationConfig();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDocumentation();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
