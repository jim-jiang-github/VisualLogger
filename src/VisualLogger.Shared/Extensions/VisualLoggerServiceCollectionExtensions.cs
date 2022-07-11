
using BootstrapBlazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Extensions;

namespace VisualLogger.Shared.Extensions
{
    public static class VisualLoggerServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualLoggerShared(this IServiceCollection services)
        {
            services.AddBootstrapBlazor();
            services.AddLocalization();
            services.AddVisualLogger();
            services.AddScoped<DialogService>();
            services.AddSingleton<MenuSideBarService>();
            services.AddSingleton<MenuTopBarService>();
            services.AddSingleton<IScenarioOptions, ScenarioOptions>();
            return services;
        }
    }
}
