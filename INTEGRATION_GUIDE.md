# 🔌 Integration Guide: Horizon Observability

This guide explains how to add professional-grade monitoring to your existing .NET 8 Web API in under 5 minutes.

## ❓ Why use this?

Most developers manually set up logging and tracing for every new project. This leads to:
1.  **Inconsistency**: Team A uses different log formats than Team B.
2.  **Blind Spots**: Database calls or external APIs aren't traced.
3.  **Complexity**: Setting up OpenTelemetry manually requires 50+ lines of "boilerplate" code.

**Horizon Observability** abstracts all of that. You call one method, and your service becomes "Cloud Native."

---

## 🛠 Step 1: Add the Reference

Add the `Horizon.Observability` project to your solution.

```bash
# In your terminal, from your project folder:
dotnet add reference ../path/to/Horizon.Observability.csproj
```

## 🛠 Step 2: Configure Settings

Add this section to your `appsettings.json`. Replace `MyCoolService` with your actual service name.

```json
{
  "HorizonObservability": {
    "ServiceName": "MyCoolService",
    "SeqUrl": "http://localhost:5341",
    "OtlpEndpoint": "http://localhost:4317",
    "EnableConsoleLogging": true,
    "TracingSampleRate": 1.0
  }
}
```

## 🛠 Step 3: Initialize in Program.cs

Open `Program.cs` and add the following **3 lines**:

```csharp
using Horizon.Observability.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Register the SDK ---
builder.Services.AddHorizonObservability(builder.Configuration);

var app = builder.Build();

// --- 2. Map Health Checks (/health/live and /health/ready) ---
app.MapHorizonHealthChecks();

// --- 3. Enable the Middleware (Correlation IDs & Problem Details) ---
app.UseHorizonObservability();

app.Run();
```

---

## ✅ How to Verify it's working?

1.  **Check startup logs**: You should see a JSON-formatted log in your console.
2.  **Hit a Health Check**: Navigate to `http://localhost:YOUR_PORT/health/live`. You should see `Healthy`.
3.  **Check the Headers**: Run any request in Swagger/Postman. You will see an `X-Correlation-ID` header in the response.

## 🚀 Pro-Tip: Advanced Context
If you want to add custom data to your logs halfway through a request, just use Serilog's `LogContext`:

```csharp
using Serilog.Context;

// This "OrderID" will now appear in EVERY log line for this request.
using (LogContext.PushProperty("OrderId", "12345"))
{
    _logger.LogInformation("Processing payment...");
}
```
