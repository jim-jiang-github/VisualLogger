using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger;
using VisualLogger.Messages;
using VisualLogger.Scenarios;

namespace VisualLogger.Extensions
{
    public static class VisualLoggerServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualLogger(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                       .MinimumLevel.Debug()
                       .WriteTo.Console()
#else
           .MinimumLevel.Information()
#endif
                       .WriteTo.File("log.txt",
                       rollingInterval: RollingInterval.Day,
                       rollOnFileSizeLimit: true)
                       .CreateLogger();
            services.AddScoped<Scenario>();
            services.AddScoped<ScenarioConfig>();
            services.AddScoped<AppConfig>();
            return services;
        }
    }
}
