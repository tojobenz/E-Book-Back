# EBook API

A simple REST API for managing books and favorites, built with **ASP.NET Core 10**, **Entity Framework Core**, and **SQLite**.

The project follows a clean layered architecture and is designed as a backend for a book management application.

## 🛠️ Tech Stack

* ASP.NET Core 10
* Entity Framework Core 10
* SQLite (with persistent disk on Render)
* Swagger / OpenAPI
* FluentValidation
* Serilog
* xUnit + Moq + FluentAssertions

## 📁 Project Structure

```text
EBook.Api             → API, controllers and configuration
EBook.Application     → Services, DTOs and validation
EBook.Domain          → Entities and interfaces
EBook.Infrastructure  → Database, repositories and external APIs
```

## ✨ Features

* Book CRUD
* Search, filtering, pagination and sorting
* Add/remove favorite books
* Open Library API integration
* Request validation
* Global error handling
* Structured logging
* Swagger documentation
* Unit tests

## 🔌 Main Endpoints

### Books

```text
GET    /api/books
GET    /api/books/{id}
POST   /api/books
PUT    /api/books/{id}
DELETE /api/books/{id}
```

Example:

```text
GET /api/books?search=harry&page=1&pageSize=10&sortBy=title
```

### Favorites

```text
GET    /api/favorites
POST   /api/favorites/{bookId}
DELETE /api/favorites/{bookId}
```

### Open Library

```text
GET /api/external/search?query=harry+potter
GET /api/external/book/{isbn}
```

## 🚀 Getting Started

### Requirements

* .NET 10 SDK

### Run the project

Clone the repository, restore the packages and apply the database migrations:

```bash
dotnet restore
dotnet ef database update --project EBook.Infrastructure --startup-project EBook.Api
dotnet run --project EBook.Api
```

The API will be available at:

```text
http://localhost:5019
```

Swagger:

```text
http://localhost:5019/swagger
```

## 🧪 Tests

Run the tests with:

```bash
dotnet test
```

The tests cover the main services, validation and repository logic.

## 🚀 Deploy to Render

### Prerequisites

- A [Render](https://render.com) account
- GitHub repository with this project

### Deployment Steps

1. **Push your code to GitHub**

```bash
git add .
git commit -m "Ready for Render deployment"
git push origin main
```

2. **Create a new Web Service on Render**

- Go to [Render Dashboard](https://dashboard.render.com)
- Click "New" → "Web Service"
- Connect your GitHub repository
- Render will detect the `render.yaml` file automatically
- Click "Create Web Service"

3. **Render will automatically:**

- Build the Docker image
- Deploy it with the correct environment variables
- Set up a persistent disk (1GB) for SQLite database storage

4. **Update CORS settings**

After deployment, update `appsettings.json` to add your Render URL:

```json
"AllowedOrigins": [
  "http://localhost:4200",
  "https://your-app.onrender.com"
]
```

Then push the changes to trigger a redeployment.

### Environment Variables

Render automatically sets:
- `ASPNETCORE_URLS`: http://0.0.0.0:10000
- `ASPNETCORE_ENVIRONMENT`: Production

### Database Storage

The application uses SQLite with persistent disk storage on Render. The database file is stored at `/app/data/ebook.db` and persists across deployments.

### Database Migrations

The application uses `EnsureCreated()` for simplicity in production. For a production app, consider using EF Core migrations instead.

## 🐳 Docker Support

### Build the Docker image locally

```bash
docker build -t ebook-api .
```

### Run with Docker

```bash
docker run -p 8080:80 -e DATABASE_URL="Data Source=ebook.db" ebook-api
```

### Run with Docker Compose (for local development)

Create a `docker-compose.yml`:

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - DATABASE_URL=Data Source=ebook.db
    volumes:
      - ./ebook.db:/app/ebook.db
```

Then run:

```bash
docker-compose up
```

The API will be available at `http://localhost:8080`

This project was built as a backend practice project to work with **ASP.NET Core, EF Core, REST APIs and Clean Architecture**.
