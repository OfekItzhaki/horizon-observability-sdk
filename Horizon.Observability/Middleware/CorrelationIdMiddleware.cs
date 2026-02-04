using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Horizon.Observability.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = GetCorrelationId(context);

        // Add to response header
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        // Add to Serilog LogContext
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        // 1. Check if we already have a correlation ID in the request headers
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId) && !string.IsNullOrWhiteSpace(correlationId.ToString()))
        {
            return correlationId.ToString();
        }

        // 2. Check if we have an active Activity (OpenTelemetry)
        var activity = System.Diagnostics.Activity.Current;
        if (activity != null && !string.IsNullOrEmpty(activity.TraceId.ToHexString()))
        {
            return activity.TraceId.ToHexString();
        }

        // 3. Fallback to a new GUID
        return Guid.NewGuid().ToString();
    }
}

