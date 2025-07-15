# 📝 BlogApp API (.NET 8)

A production-ready, scalable ASP.NET Core Web API for blogging — following clean architecture with EF Core, background job processing, JWT auth, CI/CD, Docker containerization, and Azure deployment.

## 📝 Table of Contents

* [Swagger UI](#-swagger)
* [🌟 Features](#-features)
* [🛠️ Prerequisites](#️-prerequisites)
* [🚀 Setup & Installation](#-setup--installation)
* [🏗️ Project Structure](#️-project-structure)
* [🔧 Configuration](#-configuration)
* [▶️ Running the Application](#️-running-the-application)
* [🧪 Testing](#-testing)
* [🚀 Deployment](#-deployment)
* [📝 API Documentation](#-api-documentation)
* [🔍 Monitoring & Logging](#-monitoring--logging)
* [🐛 Troubleshooting](#-troubleshooting)

### 🧭 Swagger UI
- API comes with Swagger/OpenAPI support for testing and documentation.
  
![Swagger UI Screenshot](https://github.com/user-attachments/assets/54c35011-6212-479c-9d8c-8d26a02309cb)

## 🌟 Features

### Core Architecture
- **3-Tier Architecture** (Presentation, Business Logic, Data Access)
- **Repository Pattern** with EF Core
- **Clean Code** principles implementation

### Technical Components
- **Entity Framework Core** for database operations
- **AutoMapper** for DTO mapping
- **Hangfire** for background jobs (email queueing)
- **xUnit** for unit & integration testing
- **Swagger** with API documentation
- **API Versioning** (v1, v2 support)

### Security & Auth
- **JWT Authentication**
- **Email Verification**
- **.NET User Secrets** for sensitive configs
- Role-based authorization

### Operational Excellence
- **Serilog** for structured logging
- **Health Checks** (database & app health)
- Global **Error Handling**
- **Filtering & Pagination** support

### Deployment
- **Docker** containerization (API + DB)
- **GitHub Actions CI/CD**
- **Azure Deployment** (ACR, Azure SQL, Monitoring)

## 🛠️ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for deployment)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express)
- [Postman](https://www.postman.com/downloads/) (for API testing)

## 🚀 Setup & Installation

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/johntarus/DotnetBlogApp.git
   cd DotnetBlogApp
   ```

2. **Set up user secrets**
   ```bash
   cd src/BlogApp.API
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=BlogAppDb;Integrated Security=true;TrustServerCertificate=true"
   dotnet user-secrets set "JwtSettings:Key" "your-super-secret-key-here"
   dotnet user-secrets set "JwtSettings:Issuer" "BlogApp"
   dotnet user-secrets set "JwtSettings:Audience" "BlogApp"
   dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
   dotnet user-secrets set "EmailSettings:SmtpPort" "587"
   dotnet user-secrets set "EmailSettings:SmtpUsername" "your-email@gmail.com"
   dotnet user-secrets set "EmailSettings:SmtpPassword" "your-app-password"
   ```

3. **Update database**
   ```bash
   dotnet ef database update -p src/BlogApp.Infrastructure -s src/BlogApp.API
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/BlogApp.API
   ```

The API will be available at `https://localhost:5001` and Swagger UI at `https://localhost:5001/swagger`

## 🏗️ Project Structure

```
DotnetBlogApp/
├── .github/
│   └── workflows/
│       └── build-test-deploy.yml    # CI/CD Pipeline
├── src/
│   ├── BlogApp.API/                 # Presentation Layer (Web API)
│   ├── BlogApp.Core/                # Business Logic Layer
│   ├── BlogApp.Domain/              # Domain Models & Entities
│   └── BlogApp.Infrastructure/      # Data Access Layer
├── BlogApp.Tests/                   # Unit & Integration Tests
│   ├── Controllers/                 # API Controller Tests
│   ├── Helpers/                     # Test Helper Classes
│   ├── Repositories/                # Repository Tests
│   ├── Services/                    # Service Layer Tests
│   └── Utils/                       # Utility Tests
├── .dockerignore                    # Docker ignore file
├── .gitignore                       # Git ignore file
├── .env.example                     # Environment variables template
├── BlogApp.sln                      # Solution file
├── compose.yaml                     # Docker Compose configuration
├── init.sql                         # Database initialization script
└── BlogApp.sln.DotSettings.user     # User settings
```

## ▶️ Running the Application

### Development Mode

```bash
# Run the API
dotnet run --project src/BlogApp.API

# Run with hot reload
dotnet watch run --project src/BlogApp.API
```

### Production Mode

```bash
# Build the application
dotnet build -c Release

# Run the published app
dotnet run -c Release --project src/BlogApp.API
```

The API will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger`
- **Hangfire Dashboard**: `https://localhost:5001/hangfire`

## 🐳 Docker Setup

### Using Docker Compose

1. **Set up environment variables**
   ```bash
   cp .env.example .env
   # Edit .env file with your configuration
   ```

2. **Build and run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

This will start:
- BlogApp API on `https://localhost:5001/swagger`
- SQL Server on `localhost:1433`
- Hangfire Dashboard on `https://localhost:5001/hangfire`

### Manual Docker Commands

1. **Build the image**
   ```bash
   docker build -t blogapp .
   ```

2. **Run SQL Server container**
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
   ```

3. **Run the application**
   ```bash
   docker run -p 5001:80 blogapp
   ```

## 🧪 Testing

The project includes comprehensive tests organized by layers:

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Categories
```bash
# Controller tests
dotnet test --filter "Category=Controllers"

# Service tests  
dotnet test --filter "Category=Services"

# Repository tests
dotnet test --filter "Category=Repositories"

# Helper/Utility tests
dotnet test --filter "Category=Helpers"
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure
- **Controllers/**: API endpoint testing
- **Services/**: Business logic testing  
- **Repositories/**: Data access testing
- **Helpers/**: Utility and helper class testing
- **Utils/**: General utility testing

## 📊 Health Checks
<img width="1512" height="425" alt="image" src="https://github.com/user-attachments/assets/6b66f41f-d106-4a83-952d-855b7057ced5" />

Health check endpoints:
- `/health` - Overall application health
- `/healthcheck-ui` - Health check ui dashboard

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Development` |
| `ConnectionStrings__DefaultConnection` | Database connection string | - |
| `JwtSettings__Key` | JWT signing key | - |
| `JwtSettings__Issuer` | JWT issuer | `BlogApp` |
| `JwtSettings__Audience` | JWT audience | `BlogApp` |
| `EmailSettings__SmtpServer` | SMTP server | - |
| `EmailSettings__SmtpPort` | SMTP port | `587` |
| `EmailSettings__SmtpUsername` | SMTP username | - |
| `EmailSettings__SmtpPassword` | SMTP password | - |

## 🚀 Deployment

### Azure Deployment

1. **Create Azure resources**
   ```bash
   # Create resource group
   az group create --name BlogAppRG --location "East US"
   
   # Create Azure SQL Database
   az sql server create --name blogapp-sql-server --resource-group BlogAppRG --location "East US" --admin-user sqladmin --admin-password YourPassword123!
   az sql db create --resource-group BlogAppRG --server blogapp-sql-server --name BlogAppDb --service-objective Basic
   
   # Create Azure Container Registry
   az acr create --resource-group BlogAppRG --name blogappacr --sku Basic
   
   # Create Azure Container Instance
   az container create --resource-group BlogAppRG --name blogapp --image blogappacr.azurecr.io/blogapp:latest --cpu 1 --memory 1 --registry-username blogappacr --registry-password <password> --ports 80
   ```

2. **Build and push Docker image**
   ```bash
   docker build -t blogappacr.azurecr.io/blogapp:latest .
   docker push blogappacr.azurecr.io/blogapp:latest
   ```

### GitHub Actions CI/CD

The repository includes a comprehensive GitHub Actions workflow (`build-test-deploy.yml`) that:
- Builds the application
- Runs all test suites (Controllers, Services, Repositories, Helpers, Utils)
- Performs code coverage analysis
- Builds and scans Docker image for vulnerabilities
- Pushes to Azure Container Registry
- Deploys to Azure Container Instances
- Runs health checks post-deployment

Set up the following secrets in your GitHub repository:
- `AZURE_CREDENTIALS`
- `AZURE_REGISTRY_LOGIN_SERVER`
- `AZURE_REGISTRY_USERNAME`
- `AZURE_REGISTRY_PASSWORD`
- `AZURE_WEBAPP_URL`

## 📝 API Documentation

### Blog Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/posts` | Get all posts (with pagination) |
| GET | `/api/v1/posts/{id}` | Get post by ID |
| POST | `/api/v1/posts` | Create new post |
| PUT | `/api/v1/posts/{id}` | Update post |
| DELETE | `/api/v1/posts/{id}` | Delete post |

### Query Parameters

- `PageNumber` - Page number (default: 1)
- `PageSize` - Items per page (default: 10)
- `SearchQuery` - Search term
- `UserId` - UserId

## 🔍 Monitoring & Logging

### Logging

The application uses Serilog for structured logging with the following sinks:
- Console
- File

### Hangfire Dashboard

Monitor background jobs at `/hangfire` (requires authentication)

## 🐛 Troubleshooting

### Common Issues

#### Database Connection Issues
```bash
# Check SQL Server is running
docker ps

# Reset database
dotnet ef database drop -p src/BlogApp.Infrastructure -s src/BlogApp.API
dotnet ef database update -p src/BlogApp.Infrastructure -s src/BlogApp.API
```

#### Port Already in Use
```bash
# Find process using port 5001
netstat -ano | findstr :5001

# Kill the process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

#### Docker Issues
```bash
# Clean up Docker
docker system prune -a

# Rebuild containers
docker-compose down
docker-compose up --build
```

#### User Secrets Not Found
```bash
# Reinitialize user secrets
dotnet user-secrets clear --project src/BlogApp.API
dotnet user-secrets init --project src/BlogApp.API
# Then re-add all secrets
```

### Logs Location
- **Development**: Console output
- **Production**: `/app/logs/` (in Docker container)
- **Hangfire**: Available in Hangfire dashboard

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Hangfire](https://www.hangfire.io/)
- [Serilog](https://serilog.net/)

## 📞 Support

For support, email tarusjohn96@gmail.com or create an issue in this repository.

---

**Built with ❤️ using .NET 8**
