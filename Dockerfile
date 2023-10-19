#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Website/OpenTelemetryDemo/OpenTelemetryDemo.csproj", "src/Website/OpenTelemetryDemo/"]
COPY ["src/Database/OpenTelemetryDemo.EF/OpenTelemetryDemo.EF.csproj", "src/Database/OpenTelemetryDemo.EF/"]
COPY ["src/Database/OpenTelemetryDemo.Repositories.Interfaces/OpenTelemetryDemo.Repositories.Interfaces.csproj", "src/Database/OpenTelemetryDemo.Repositories.Interfaces/"]
COPY ["src/Database/OpenTelemetryDemo.Repositories/OpenTelemetryDemo.Repositories.csproj", "src/Database/OpenTelemetryDemo.Repositories/"]
COPY ["src/OpenTelmetryDemo.Shared/OpenTelemetryDemo.Shared.csproj", "src/OpenTelmetryDemo.Shared/"]
COPY ["src/Database/OpenTelemetryDemo.Services.Interfaces/OpenTelemetryDemo.Services.Interfaces.csproj", "src/Database/OpenTelemetryDemo.Services.Interfaces/"]
COPY ["src/Database/OpenTelemetryDemo.Services/OpenTelemetryDemo.Services.csproj", "src/Database/OpenTelemetryDemo.Services/"]
RUN dotnet restore "src/Website/OpenTelemetryDemo/OpenTelemetryDemo.csproj"
COPY . .
WORKDIR "/src/src/Website/OpenTelemetryDemo"
RUN dotnet build "OpenTelemetryDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenTelemetryDemo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetryDemo.dll"]