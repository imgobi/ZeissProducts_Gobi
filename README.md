# ZeissProductsChallenge
to build an API that manages products.
------------------------------------------------------------------------
Tech Stack

-   .NET 8 Web API
-   Entity Framework Core
-   MS SQL Server
-   Swagger / OpenAPI
-   xUnit for unit tests

------------------------------------------------------------------------
Products API

A .NET 8 Web API that manages products with stock handling, built using
Entity Framework Core with a SQL Server backend.
It supports CRUD operations along with stock increment/decrement
functionality, and is fully tested with xUnit unit tests.

------------------------------------------------------------------------

Project Flow

1. API Endpoints

-   POST /api/products → Create a new product
-   GET /api/products → Retrieve all products
-   GET /api/products/{id} → Retrieve a specific product
-   PUT /api/products/{id} → Update product details
-   DELETE /api/products/{id} → Delete a product
-   PUT /api/products/add-to-stock/{id}/{quantity} → Increase product
    stock
-   PUT /api/products/decrement-stock/{id}/{quantity} → Decrease product
    stock

Every product has a unique ProductId generated from a custom SQL
sequence.
Stock availability is always returned in all product endpoints.

------------------------------------------------------------------------

2. Database & EF Core Flow

-   Product IDs are generated from a SQL sequence (ProductIdSequence)
    instead of IDENTITY.
-   EF Core migrations manage schema evolution
-   Unit tests use an InMemory EF Core provider for fast execution. (Moq/ Specflow are other good options)

------------------------------------------------------------------------

3. Unit Testing Flow

-   All controller actions are tested with xUnit.
-   Repository/DbContext are mocked using InMemoryDatabase.
-   Scenarios tested include:
    -   Creating a product
    -   Getting product(s)
    -   Updating product
    -   Deleting product
    -   Increasing stock
    -   Decreasing stock
    -   Handling invalid requests (e.g., product not found)

------------------------------------------------------------------------

Design Patterns Used

Repository Pattern (via EF Core DbContext)
    -   Abstracts database access through ProductsContext, making the
        controller code clean and testable.
Dependency Injection (DI)
    -   ASP.NET Core’s built-in DI container injects ProductsContext and
        controller dependencies.
Unit of Work (via DbContext)
    -   EF Core SaveChangesAsync() ensures atomic updates across
        multiple entities.
DTO Pattern (lightweight)
    -   Input/output models in API requests keep separation between
        persistence model and exposed contract.

------------------------------------------------------------------------

Future Enhancements
-   Add other useful columns to the table (RowVersion, CreatedById, CreatedTime, UpdatedById, UpdatedTime) for Concurrency Control, Auditing. 
-   Add integration tests using WebApplicationFactory<TestStartup>/ HealthCheck.
-   Add global exception handling middleware.
-   Use FluentValidation for model validation.
-   Add JWT/ OAuth for secure endpoints.
