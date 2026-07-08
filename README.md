# Entreprenly Platform - Web Services

RESTful API backend for the Entreprenly application, built with **Domain-Driven Design (DDD)** and **CQRS**.

## Tech Stack

- **Language:** C# / .NET 10
- **Framework:** ASP.NET Core
- **ORM:** Entity Framework Core 10 (MySQL provider)
- **Database:** MySQL 8+ (Google Cloud SQL in production)
- **Security:** JWT authentication (BCrypt password hashing)
- **Documentation:** Swashbuckle (Swagger UI)

## Architecture

Bounded contexts under `src/Entreprenly.WebServices`:

| Context | Responsibility |
|---|---|
| `Iam` | Identity & Access Management: users, roles, authentication, JWT |
| `Inventory` | Products and lots (unit / weight), stock alerts |
| `Sales` | Sales |
| `Subscription` | Plans, billing, payment confirmation |
| `Profiles` | User profile, preferences, notification settings |
| `Chatbot` | WhatsApp sessions, conversations, chat messages and orders |
| `Shared` | Domain bases, persistence, OpenAPI, i18n, error handling |

Each context is layered as `Domain` / `Application` / `Infrastructure` / `Interfaces`.

## Getting Started

### Prerequisites

- .NET 10 SDK
- MySQL 8+ (a local instance for development)
- Docker (optional, for containerized runs)

### Configuration

Copy `src/Entreprenly.WebServices/appsettings.Development.json.example` to
`appsettings.Development.json` and set your local MySQL connection string and secrets.

### Run

```bash
# Restore dependencies
dotnet restore

# Development
dotnet run --project src/Entreprenly.WebServices

# Build and publish
dotnet publish src/Entreprenly.WebServices -c Release -o ./publish

# Run with Docker
docker build -t ap-entreprenly .
docker run -p 8080:8080 ap-entreprenly
```

The API is served under `/api/v1` and Swagger UI is available at `/swagger`.

## Deployment

The service is deployed to **Google Cloud Run**. A Cloud Build trigger builds the
container from the `Dockerfile` and rolls out a new revision automatically on every
push to `main` (no manual steps required).

In production the app connects to **Cloud SQL (MySQL)** through the Cloud Run Unix
socket (`/cloudsql/<project>:<region>:<instance>`), so no public IP or host/port is
configured. The Cloud Run service is set up with:

- Environment variables: `ASPNETCORE_ENVIRONMENT=Production`, `DATABASE_PROJECT`,
  `DATABASE_REGION`, `DATABASE_INSTANCE`, `DATABASE_NAME`, `DATABASE_USER`,
  `DATABASE_PASSWORD`, `TokenSettings__Secret`.
- The Cloud SQL instance attached to the service, and the runtime service account
  granted the `roles/cloudsql.client` role.

## License

See [LICENSE](LICENSE).
