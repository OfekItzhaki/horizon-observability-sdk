FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["demo/Horizon.Platform.Demo/Horizon.Platform.Demo.csproj", "demo/Horizon.Platform.Demo/"]
COPY ["Horizon.Platform.Sdk/Horizon.Platform.Sdk.csproj", "Horizon.Platform.Sdk/"]
RUN dotnet restore "demo/Horizon.Platform.Demo/Horizon.Platform.Demo.csproj"
COPY . .
WORKDIR "/src/demo/Horizon.Platform.Demo"
RUN dotnet build "Horizon.Platform.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Horizon.Platform.Demo.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Horizon.Platform.Demo.dll"]
