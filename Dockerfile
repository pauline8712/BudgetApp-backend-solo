# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (for layer caching)
COPY BudgetApp.sln .
COPY BudgetApp.API/BudgetApp.API.csproj BudgetApp.API/
COPY BudgetApp.Application/BudgetApp.Application.csproj BudgetApp.Application/
COPY BudgetApp.Domain/BudgetApp.Domain.csproj BudgetApp.Domain/
COPY BudgetApp.Infrastructure/BudgetApp.Infrastructure.csproj BudgetApp.Infrastructure/
COPY BudgetApp.Tests/BudgetApp.Tests.csproj BudgetApp.Tests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish BudgetApp.API/BudgetApp.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BudgetApp.API.dll"]
