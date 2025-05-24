# AegisPraxis Nexus API

This is the backend API for the AegisPraxis Nexus telemedicine platform.
It follows Clean Architecture principles and Domain-Driven Design (DDD) using ASP.NET Core 8.

## Architecture

AegisPraxis.Api              → Application entry point (Web API)
AegisPraxis.Infrastructure   → External concerns (DB, Keycloak, email, etc)
AegisPraxis.Application      → Application logic, use cases
AegisPraxis.Domain           → Domain entities, interfaces, business rules

## Authentication

- Uses Keycloak (managed in the `Heimdall` repo) as the Identity Provider.
- Multi-tenant support via one realm per clinic (e.g. `clinic-Blue-Star`, `clinic-Green-Star`)
- Supports Google OAuth, Facebook login and role-based authorization.

## Technologies

- ASP.NET Core 8 Web API
- Clean Architecture + DDD
- Keycloak (JWT + OAuth2)
- PostgreSQL (planned)
- Docker (dev/test environments)

## Running the project

> Work in progress — Docker configuration and migrations coming soon.

## Folder Structure

src/
├── AegisPraxis.Api/            # Web API layer (controllers, filters)
├── AegisPraxis.Infrastructure/ # DB, Keycloak, external services
├── AegisPraxis.Application/    # Application logic (use cases, DTOs)
└── AegisPraxis.Domain/         # Domain entities and interfaces

tests/
└── AegisPraxis.Tests/          # Unit and integration tests

## Roadmap

- [x] Project setup
- [ ] JWT Authentication (Keycloak integration)
- [ ] Multi-tenant support
- [ ] Admin and User modules
- [ ] Docker dev environment
- [ ] Swagger docs and CI

## License

This project is open-source and available under the MIT License.
