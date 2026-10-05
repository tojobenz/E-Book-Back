# EBook API

A simple REST API for managing books and favorites, built with **ASP.NET Core 10**, **Entity Framework Core**, and **SQLite**.

The project follows a clean layered architecture and is designed as a backend for a book management application.

## 🛠️ Tech Stack

* ASP.NET Core 10
* Entity Framework Core
* SQLite
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

This project was built as a backend practice project to work with **ASP.NET Core, EF Core, REST APIs and Clean Architecture**.
