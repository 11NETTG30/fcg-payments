using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace API.Configurations;

public static class OpenTelemetryConfiguration
{
    extension(IServiceCollection services)
    {
        public void AddTelemetry(WebApplicationBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]) ||
                string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]) ||
                string.IsNullOrWhiteSpace(builder.Configuration["OTEL_SERVICE_NAME"]))
                return;

            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(builder.Configuration["OTEL_SERVICE_NAME"]!))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!);

                            options.Headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"];
                        });
                });
        }
    }
}
