# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files (excluding tests)
COPY ["EBook.Api/EBook.Api.csproj", "EBook.Api/"]
COPY ["EBook.Application/EBook.Application.csproj", "EBook.Application/"]
COPY ["EBook.Domain/EBook.Domain.csproj", "EBook.Domain/"]
COPY ["EBook.Infrastructure/EBook.Infrastructure.csproj", "EBook.Infrastructure/"]

# Restore dependencies for API project only
RUN dotnet restore "EBook.Api/EBook.Api.csproj"

# Copy all source files (excluding tests)
COPY EBook.Api/. EBook.Api/
COPY EBook.Application/. EBook.Application/
COPY EBook.Domain/. EBook.Domain/
COPY EBook.Infrastructure/. EBook.Infrastructure/

# Build the project
WORKDIR "/src/EBook.Api"
RUN dotnet build "EBook.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "EBook.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install SQLite support
RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

# Create data directory for persistent storage
RUN mkdir -p /app/data

COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "EBook.Api.dll"]
