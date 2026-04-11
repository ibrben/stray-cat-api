# StrayCat API

A .NET 10.0 Web API for managing trips, bookings, and user authentication with Google OAuth integration.

## Features

- **Trip Management**: Create, read, update, and delete trips
- **Booking System**: Manage trip bookings with reference codes
- **Authentication**: JWT-based authentication with Google OAuth support
- **Database**: PostgreSQL with Entity Framework Core
- **Containerized**: Docker support for easy deployment

## Technology Stack

- **.NET 10.0** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with PostgreSQL
- **JWT Authentication** - Token-based authentication
- **Google OAuth** - External authentication provider
- **Docker** - Containerization

## Quick Start with Docker

### Prerequisites

- Docker and Docker Compose installed
- Git

### Running the Application

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd stray-cat-api/StrayCat
   ```

2. **Build and run with Docker Compose**:
   ```bash
   docker-compose up --build
   ```

3. **Access the API**:
   - API URL: `http://localhost:5000`
   - Database: `localhost:5432`

### Docker Compose Services

- **postgres**: PostgreSQL 15 database
- **api**: StrayCat API application

### Environment Variables

The application uses the following environment variables (can be configured in `appsettings.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=straycat;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "YourSecretKeyHere",
    "Issuer": "StrayCatAPI",
    "Audience": "StrayCatClient"
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-google-client-secret"
    }
  },
  "Frontend": {
    "Url": "http://localhost:3000"
  }
}
```

## Development Setup

### Prerequisites

- .NET 10.0 SDK
- PostgreSQL 15
- Visual Studio Code or Visual Studio

### Local Development

1. **Install dependencies**:
   ```bash
   dotnet restore
   ```

2. **Update database connection** in `appsettings.json`

3. **Run database migrations**:
   ```bash
   dotnet ef database update --project StrayCat.Infrastructure/StrayCat.Infrastructure.csproj
   ```

4. **Run the application**:
   ```bash
   dotnet run --project StrayCat.API
   ```

## API Endpoints

### Authentication

- `POST /auth/login` - User login
- `GET /auth/google` - Initiate Google OAuth
- `GET /auth/google/callback` - Google OAuth callback
- `GET /auth/me` - Get current user info

### Trips

- `GET /api/trips` - Get all trips
- `GET /api/trips/{id}` - Get trip by ID
- `POST /api/trips` - Create new trip
- `PUT /api/trips/{id}` - Update trip
- `DELETE /api/trips/{id}` - Delete trip

### Bookings

- `GET /api/bookings` - Get all bookings
- `POST /api/bookings` - Create new booking
- `PUT /api/bookings/{id}` - Update booking
- `DELETE /api/bookings/{id}` - Delete booking

## Docker Commands

### Build Image
```bash
docker build -t straycat-api .
```

### Run Container
```bash
docker run -d -p 5000:80 --name straycat-api-container straycat-api
```

### Stop Container
```bash
docker stop straycat-api-container
```

### Remove Container
```bash
docker rm straycat-api-container
```

## Production Deployment

### Environment Variables for Production

Set these environment variables in your production environment:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=your-db-host;Port=5432;Database=straycat;Username=your-user;Password=your-password
Jwt__Key=YourProductionSecretKey
Jwt__Issuer=StrayCatAPI
Jwt__Audience=StrayCatClient
Authentication__Google__ClientId=your-production-google-client-id
Authentication__Google__ClientSecret=your-production-google-client-secret
Frontend__Url=https://your-frontend-domain.com
```

### Docker Compose Production

```bash
docker-compose -f docker-compose.yml up -d
```

## Database Migrations

### Create New Migration
```bash
dotnet ef migrations add MigrationName --project StrayCat.Infrastructure/StrayCat.Infrastructure.csproj
```

### Apply Migrations
```bash
dotnet ef database update --project StrayCat.Infrastructure/StrayCat.Infrastructure.csproj
```

## Architecture

The application follows Clean Architecture principles:

- **StrayCat.Domain**: Core business entities
- **StrayCat.Application**: Business logic and services
- **StrayCat.Infrastructure**: Data access and external services
- **StrayCat.API**: Web API controllers and configuration

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.
