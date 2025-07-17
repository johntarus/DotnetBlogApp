## BlogApp API (.NET 8)
### Key Features
- JWT Authentication

- Email Verification

- Background Job Processing

- API Versioning

- Docker Support

- Health Monitoring

- Automated Testing

### Setup & Installation

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

3. **Run the application**
   ```bash
   dotnet run --project src/BlogApp.API
   ```

The API will be available at:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger`
- **Hangfire Dashboard**: `https://localhost:5001/hangfire`

### Docker Setup

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

### Testing
```bash
dotnet test
```

### Deployment

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

### Support

For support, email tarusjohn96@gmail.com or create an issue in this repository.
