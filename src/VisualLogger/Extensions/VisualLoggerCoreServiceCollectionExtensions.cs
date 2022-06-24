using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger;
using VisualLogger.Scenarios;

namespace VisualLogger.Extensions
{
    public static class VisualLoggerCoreServiceCollectionExtensions
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
            services.AddSingleton<Scenario>();
            services.AddSingleton<ScenarioConfig>();
            services.AddSingleton<AppConfig>();
            return services;
        }
    }
}
