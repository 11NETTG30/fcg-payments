using API.Configurations;
using Application.Configuration;
using Application.DependencyInjection;
using DotNetEnv;
using FCG.Shared.Infrastructure.Configurations;
using Infrastructure.DependencyInjection;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddDocumentation();
builder.Services.AddDb(builder.Configuration);
builder.Services.AddDI(builder.Configuration);
builder.AddObservabilidade();

builder.Services
    .AddApplication()
    .AddFluentValidationConfig();

builder.Services.Configure<PagamentoOptions>(
    builder.Configuration.GetSection(PagamentoOptions.SectionName)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDocumentation();
}

await app.UseMigrationsAsync();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();