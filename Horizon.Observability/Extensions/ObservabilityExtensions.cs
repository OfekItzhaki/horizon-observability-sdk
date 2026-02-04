using Horizon.Observability.Middleware;
using Horizon.Observability.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Horizon.Observability.Extensions;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddHorizonObservability(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind options
        services.Configure<ObservabilityOptions>(configuration.GetSection(ObservabilityOptions.SectionName));
        var options = configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>() ?? new ObservabilityOptions();

        var serviceName = string.IsNullOrWhiteSpace(options.ServiceName) 
            ? AppDomain.CurrentDomain.FriendlyName 
            : options.ServiceName;

        // 0. Problem Details (RFC 7807)
        services.AddProblemDetails();

        // 1. Configure Serilog
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics.HealthChecks", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", serviceName)
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId();

        if (options.EnableConsoleLogging)
        {
            loggerConfiguration.WriteTo.Console(new JsonFormatter());
        }

        if (!string.IsNullOrWhiteSpace(options.SeqUrl))
        {
            loggerConfiguration.WriteTo.Seq(options.SeqUrl);
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(dispose: true);
        });

        // 2. Configure OpenTelemetry
        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(serviceName)
                        .AddAttributes(new[] { new KeyValuePair<string, object>("deployment.environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production") }))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(options.TracingSampleRate)))
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(options.OtlpEndpoint);
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("Horizon.Observability") // Custom Meter for business metrics
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(options.OtlpEndpoint);
                    });
            });

        // 3. Health Checks
        services.AddHorizonHealthChecks();

        return services;
    }

    public static IApplicationBuilder UseHorizonObservability(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(); // Use Problem Details
        app.UseStatusCodePages();   // Use Problem Details for 4xx/5xx
        
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }
}


