using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Core;
using VisualLogger.Core.Scenarios;

namespace VisualLogger.Extensions
{
    public static class VisualLoggerCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualLoggerCore(this IServiceCollection services)
        {
            services.AddSingleton<Scenario>();
            services.AddSingleton<ScenarioConfig>();
            services.AddSingleton<AppConfig>();
            return services;
        }
    }
}
