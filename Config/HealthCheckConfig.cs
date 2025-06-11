using HealthChecks.UI.Client;
using HealthChecks.UI.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace BlogApp.Config;

public class HealthCheckConfig()
{
    public static IApplicationBuilder UseHealthCheckEndpoints(IApplicationBuilder app)
    {
            app.UseHealthChecks("/api/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI(options => { options.UIPath = "/healthcheck-ui"; });
            return app;
    }
}