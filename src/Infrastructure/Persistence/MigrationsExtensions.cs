using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class MigrationsExtensions
{
    extension(WebApplication app)
    {
        public void UseMigrations()
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PagamentoDbContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
