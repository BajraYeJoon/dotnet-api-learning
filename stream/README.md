# Stream API

This project follows Clean Architecture principles to create a maintainable and scalable API.

## Project Structure

```
src/
├── API/                 # API Layer: Controllers, Middleware, and API-specific concerns
│   ├── Controllers/     # API Controllers
│   ├── Middleware/     # Custom Middleware
│   └── Program.cs      # Application Entry Point
├── Core/               # Core Layer: Business Logic and Domain
│   ├── Entities/      # Domain Entities
│   ├── Interfaces/    # Interfaces/Contracts
│   ├── Exceptions/    # Custom Exceptions
│   └── DTOs/          # Data Transfer Objects
└── Infrastructure/    # Infrastructure Layer: External Concerns
    ├── Data/          # Database Context and Configurations
    ├── Services/      # Implementation of Core Interfaces
    ├── Identity/      # Authentication and Authorization
    └── Persistence/   # Database Related Code

tests/
├── Unit/             # Unit Tests
└── Integration/      # Integration Tests
```

## Architecture Overview

This project follows Clean Architecture principles:

1. **Core Layer**: Contains business logic and domain entities. It has no dependencies on other layers.
2. **Infrastructure Layer**: Implements interfaces defined in the Core layer.
3. **API Layer**: Handles HTTP requests and responses, using services defined in Core and implemented in Infrastructure.

## Getting Started

1. Clone the repository
2. Ensure you have .NET 7.0+ installed
3. Run `dotnet restore` to restore dependencies
4. Update the connection string in `src/API/appsettings.json`
5. Run `dotnet ef database update` to apply migrations
6. Run `dotnet run --project src/API/API.csproj` to start the application

## Development Guidelines

1. Keep the Core layer independent of external concerns
2. Use interfaces to define contracts between layers
3. Follow SOLID principles
4. Write unit tests for business logic
5. Use meaningful names for classes and methods
6. Document public APIs
7. Handle exceptions appropriately
8. Use dependency injection
9. Follow REST principles for API endpoints
10. Use async/await for I/O operations

## Authentication

The API uses JWT tokens for authentication:
1. Users can register and login to receive tokens
2. Access tokens are short-lived
3. Refresh tokens can be used to get new access tokens
4. Email verification is required for new accounts

## Rate Limiting

The API implements rate limiting to prevent abuse:
1. General rate limiting for all endpoints
2. Stricter limits for authentication endpoints
3. Custom limits for specific actions

## Contributing

1. Create a feature branch
2. Make your changes
3. Write/update tests
4. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details 