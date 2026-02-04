# Horizon Platform Demo Starter
Write-Host "🚀 Starting Horizon Platform Stack..." -ForegroundColor Cyan

# 1. Check if Docker is running
docker info >$null 2>&1
if ($LastExitCode -ne 0) {
    Write-Host "❌ Error: Docker is not running! Please start Docker Desktop and try again." -ForegroundColor Red
    exit
}

# 2. Run Docker Compose
docker-compose -f "$PSScriptRoot/docker-compose.yaml" up -d --build

if ($LastExitCode -eq 0) {
    Write-Host "`n✅ Success! The stack is coming online." -ForegroundColor Green
    Write-Host "------------------------------------------"
    Write-Host "📊 Grafana: http://localhost:3001 (admin/admin)"
    Write-Host "📝 Seq (Logs): http://localhost:8081"
    Write-Host "🔍 Jaeger (Traces): http://localhost:16686"
    Write-Host "🌐 Demo API: http://localhost:5000/swagger"
    Write-Host "------------------------------------------"
    Write-Host "Generating some initial traffic for you..."
    Invoke-WebRequest -Uri "http://localhost:5000/simulation/success" -UseBasicParsing >$null 2>&1
    Write-Host "Done! Head over to Grafana."
} else {
    Write-Host "❌ Failed to start the stack. Check the logs above." -ForegroundColor Red
}
