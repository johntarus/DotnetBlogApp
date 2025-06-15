using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlogApp.Infrastructure.HealthChecks;

public class RemoteHealthCheck(IHttpClientFactory httpClientFactory) : IHealthCheck
{
    private const string RemoteHealthCheckUrl = "https://www.google.com/";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient("healthCheck");

            httpClient.Timeout = TimeSpan.FromSeconds(5);

            var response = await httpClient.GetAsync(RemoteHealthCheckUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Remote endpoint is healthy.");
            }

            return HealthCheckResult.Unhealthy($"Remote endpoint returned status code {response.StatusCode}.");
        }
        catch (TaskCanceledException)
        {
            return HealthCheckResult.Unhealthy("Remote endpoint timed out.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Exception occurred while checking remote endpoint: {ex.Message}");
        }
    }
}