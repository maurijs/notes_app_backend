# --- Fase 1: Build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["backend.csproj", "."]
COPY ["backend.sln", "."]

RUN dotnet restore "backend.sln"

COPY . .

WORKDIR /src

RUN dotnet publish "backend.csproj" -c Release -o /app/publish

# --- Fase 2: Final ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "backend.dll"]
