using Microsoft.Extensions.Hosting;
using Serilog;

namespace BlogApp.Infrastructure.Configurations;

public static class LoggingConfig
{
        public static void ConfigureLogging(this IHostBuilder host)
        {
            host.UseSerilog((context, config) => 
                config.ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext());
        }
}