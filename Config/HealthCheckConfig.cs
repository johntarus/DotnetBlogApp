using BlogApp.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlogApp.Config;

public static class HealthCheckConfig
{
    public static void AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        // Add basic health checks
        services.AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                healthQuery: "SELECT 1", 
                name: "SQL Server", 
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "Feedback", "Database" })
            .AddCheck<RemoteHealthCheck>(
                "Remote endpoint health check", 
                failureStatus: HealthStatus.Unhealthy)
            .AddCheck<MemoryHealthCheck>(
                "BlogApp service memory check", 
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "BlogApp Service" });

        // Configure Health Checks UI
        services.AddHealthChecksUI(opt =>
        {
            opt.SetEvaluationTimeInSeconds(300);
            opt.MaximumHistoryEntriesPerEndpoint(60);
            opt.SetApiMaxActiveRequests(1);
            opt.AddHealthCheckEndpoint("BlogApp API", "/api/health");
        }).AddInMemoryStorage();
    }

    public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/api/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHealthChecksUI(options => 
        {
            options.UIPath = "/healthcheck-ui";
        });

        return app;
    }
}