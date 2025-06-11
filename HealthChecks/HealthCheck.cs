using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlogApp.HealthChecks;

public static class HealthCheck
{
    public static void ConfigureHealthChecks(IServiceCollection services,IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("DefaultConnection"),
                healthQuery: "Select 1", name: "SQL Server", failureStatus: HealthStatus.Unhealthy,
                tags: new string[] { "Feedback", "Database" })
            .AddCheck<RemoteHealthCheck>("Remote endpoint is health check", failureStatus: HealthStatus.Unhealthy)
            .AddCheck<MemoryHealthCheck>("BlogApp service memory check", failureStatus: HealthStatus.Unhealthy,
                tags: new string[] { "BlogApp Service" });
            // .AddUrlGroup(new Uri("http://localhost:5263/api/heartbeats/ping"), name: "base URL",
            //     failureStatus: HealthStatus.Unhealthy);

        services.AddHealthChecksUI(opt =>
        {
            opt.SetEvaluationTimeInSeconds(60); //time in seconds between check    
            opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
            opt.SetApiMaxActiveRequests(1); //api requests concurrency    
            opt.AddHealthCheckEndpoint("BlogApp api", "/api/health"); //map health check api 
        }).AddInMemoryStorage();
    }
}