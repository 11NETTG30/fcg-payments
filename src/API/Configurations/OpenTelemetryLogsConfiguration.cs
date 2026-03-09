using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace API.Configurations;

public static class OpenTelemetryLogsConfiguration
{
    extension(IServiceCollection services)
    {
        public void AddLogsTelemetry(WebApplicationBuilder builder)
        {
            if (builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] is null ||
                builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"] is null)
                return;

            builder.Logging.ClearProviders();

            builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;

                options.AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!);
                    otlp.Headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]!;
                });
            });
        }
    }
}
