using Serilog;

namespace BlogApp.Config;

public static class LoggingConfig
{
        public static void ConfigureLogging(this IHostBuilder host)
        {
            host.UseSerilog((context, config) => 
                config.ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext());
        }
}