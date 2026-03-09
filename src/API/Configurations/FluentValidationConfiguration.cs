using FluentValidation.AspNetCore;

namespace API.Configurations;

public static class FluentValidationConfiguration
{
    public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        return services;
    }
}