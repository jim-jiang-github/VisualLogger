
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radzen;
using VisualLogger.Extensions;

namespace VisualLogger.Shared.Extensions
{
    public static class VisualLoggerServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualLoggerShared(this IServiceCollection services)
        {
            services.AddLocalization();
            services.AddVisualLogger();
            services.AddBootstrapBlazor();
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            services.AddSingleton<MenuSideBarService>();
            services.AddSingleton<MenuTopBarService>();
            services.AddSingleton<IScenarioOptions, ScenarioOptions>();
            return services;
        }
    }
}
