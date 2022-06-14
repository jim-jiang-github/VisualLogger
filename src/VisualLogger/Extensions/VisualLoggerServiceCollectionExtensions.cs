
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radzen;

namespace VisualLogger.Extensions
{
    public static class VisualLoggerServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualLogger(this IServiceCollection services)
        {
            services.AddLocalization();
            services.AddVisualLoggerCore();
            services.AddScoped<SidebarMenuService>();
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            services.AddSingleton<IScenarioOptions, SceneOptions>();
            return services;
        }
    }
}
