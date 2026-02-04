FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Horizon.Demo.API/Horizon.Demo.API.csproj", "Horizon.Demo.API/"]
COPY ["Horizon.Observability/Horizon.Observability.csproj", "Horizon.Observability/"]
RUN dotnet restore "Horizon.Demo.API/Horizon.Demo.API.csproj"
COPY . .
WORKDIR "/src/Horizon.Demo.API"
RUN dotnet build "Horizon.Demo.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Horizon.Demo.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Horizon.Demo.API.dll"]
