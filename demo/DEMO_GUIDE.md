# 📖 Quick Demo Guide

Welcome to the **Horizon Platform** demo! This guide explains how to log in and what to look for.

## 🔐 Credentials

| Service | URL | Username | Password |
| :--- | :--- | :--- | :--- |
| **Grafana** (Dashboards) | [http://localhost:3001](http://localhost:3001) | `admin` | `admin` |
| **Seq** (Logs) | [http://localhost:8081](http://localhost:8081) | None | None |
| **Jaeger** (Traces) | [http://localhost:16686](http://localhost:16686) | None | None |

> [!TIP]
> On the first login to Grafana, it will ask to change your password. You can click **"Skip"** at the bottom.

## 🏃 How to Run the Demo

### Using the scripts (Recommended)
*   **To Start**: Right-click `start-demo.ps1` and select **"Run with PowerShell"**.
*   **To Stop**: Right-click `stop-demo.ps1` and select **"Run with PowerShell"**.

### Using the command line
```powershell
# From the project root:
docker-compose -f demo/docker-compose.yaml up -d --build

# Or from inside the demo folder:
cd demo
docker-compose up -d --build
```

---

## 🚦 Testing the SDK

Once the containers are healthy, click these links to generate data:

1.  **[Success Request](http://localhost:5000/simulation/success)**: Generates a 200 OK and a custom "Checkout" metric.
2.  **[Slow Request](http://localhost:5000/simulation/slow)**: Adds 1-2 seconds of latency to see the "long span" in Jaeger.
3.  **[Error Request](http://localhost:5000/simulation/error)**: Crashes the API (500) to see how Problem Details and Errors look in the logs.

## 📊 What to Check?

1.  **Grafana**: Search for the "Horizon Observability" dashboard. You'll see real-time RPS and Latency.
2.  **Seq**: Filter by `CorrelationId` to see every log related to a single click.
3.  **Jaeger**: Find `Horizon.Platform.Demo` and look for the specific spans.
