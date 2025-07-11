using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Infrastructure.Configurations;

public static class CorsConfig
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowClients", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:3000",  // React / Next.js
                        "http://localhost:4200",  // Angular
                        "http://localhost:5173" ) // Vue / Vite
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}