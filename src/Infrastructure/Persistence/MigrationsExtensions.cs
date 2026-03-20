using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public static class MigrationsExtensions
{
    extension(WebApplication app)
    {
        public async Task UseMigrationsAsync()
        {
            const int maxTentativas = 10;
            const int esperaSegundos = 5;

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PagamentoDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<PagamentoDbContext>>();

            for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
            {
                try
                {
                    logger.LogInformation("Aplicando migrations (tentativa {Tentativa}/{Max})...", tentativa, maxTentativas);
                    await dbContext.Database.MigrateAsync();
                    logger.LogInformation("Migrations aplicadas com sucesso.");
                    return;
                }
                catch (Exception ex) when (tentativa < maxTentativas)
                {
                    logger.LogWarning(ex, "Banco indisponível. Aguardando {Segundos}s antes de tentar novamente...", esperaSegundos);
                    await Task.Delay(TimeSpan.FromSeconds(esperaSegundos));
                }
            }
        }
    }
}
