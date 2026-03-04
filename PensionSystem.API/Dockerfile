FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PensionSystem.API/PensionSystem.API.csproj", "PensionSystem.API/"]
COPY ["PensionSystem.Application/PensionSystem.Application.csproj", "PensionSystem.Application/"]
COPY ["PensionSystem.Domain/PensionSystem.Domain.csproj", "PensionSystem.Domain/"]
COPY ["PensionSystem.Infrastructure/PensionSystem.Infrastructure.csproj", "PensionSystem.Infrastructure/"]
RUN dotnet restore "PensionSystem.API/PensionSystem.API.csproj"
COPY . .
WORKDIR "/src/PensionSystem.API"
RUN dotnet build "PensionSystem.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PensionSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PensionSystem.API.dll"]
