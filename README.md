# Horizon Platform

Typically, setting up "Enterprise Grade" observability in .NET takes days of configuring NuGet packages.
**Horizon Platform** does it in **one line of code**.

It bundles **OpenTelemetry**, **Serilog**, **Health Checks**, and **Problem Details** into a single, standardized platform.

## 🚀 Features

*   **Distributed Tracing**: Native OpenTelemetry support for ASP.NET Core, HttpClient, and EF Core.
*   **Structured Logging**: Pre-configured Serilog with JSON formatting and enrichers (`CorrelationId`, `MachineName`, `Service`).
*   **Metrics**: Custom business metrics via `System.Diagnostics.Metrics`.
*   **W3C TraceContext**: Industry-standard trace propagation compatible with Jaeger, Honeycomb, and Datadog.
*   **Resilience**: Built-in Liveness (`/health/live`) and Readiness (`/health/ready`) probes.
*   **Standardization**: Enforces RFC 7807 (Problem Details) for all errors.

## 📦 Installation

Add the project reference or NuGet package:

```bash
dotnet add package Horizon.Platform.Sdk
```

## 🛠 Usage

### 1. Configuration (`appsettings.json`)
Control everything via configuration. No hardcoded URLs.

```json
{
  "HorizonObservability": {
    "ServiceName": "MyBillingService",
    "SeqUrl": "http://localhost:5341",
    "OtlpEndpoint": "http://localhost:4317",
    "EnableConsoleLogging": true,
    "TracingSampleRate": 1.0  // 0.0 to 1.0
  }
}
```

### 2. Startup (`Program.cs`)

```csharp
using Horizon.Platform.Sdk.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Services (Configuration is auto-bound)
builder.Services.AddHorizonObservability(builder.Configuration);

var app = builder.Build();

// 2. Map Health Checks
app.MapHorizonHealthChecks();

// 3. Enable Middleware
app.UseHorizonObservability();

app.Run();
```

## 🎮 Running the Demo

This repository includes a full local observability stack (Jaeger, Seq, Prometheus, Grafana).

```powershell
cd demo
docker-compose up -d --build
```

### Accessing the Tools:
*   **Start Demo**: Right-click `demo/start-demo.ps1` -> Run with PowerShell.
*   **Stop Demo**: Right-click `demo/stop-demo.ps1` -> Run with PowerShell.
*   **Grafana (Dashboards):** [http://localhost:3001](http://localhost:3001) (admin/admin)
*   **Seq (Logs):** [http://localhost:8081](http://localhost:8081)
*   **Jaeger (Traces):** [http://localhost:16686](http://localhost:16686)
*   **Demo API:** [http://localhost:5000/simulation/success](http://localhost:5000/simulation/success)

## 📊 Ownership Guide

*   **Adding Metrics**: Use `HorizonMetrics.cs` to add counters like `CheckoutsCounter.Add(1)`.
*   **Adding Instrumentation**: Update `ObservabilityExtensions.cs` to add `.AddRedisInstrumentation()` etc.
*   **Changing Exporters**: Update `demo/otel-collector-config.yaml` to point to Datadog/NewRelic instead of Jaeger.