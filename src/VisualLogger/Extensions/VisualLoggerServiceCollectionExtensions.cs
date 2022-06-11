using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.InterfaceImplModules;
using VisualLogger.InterfaceModules;

namespace VisualLogger.Extensions
{
    public static class VisualLoggerServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualLogger(this IServiceCollection services)
        {
            services.AddLocalization();
            services.AddSingleton<ISceneOptions, SceneOptions>();
            return services;
        }
    }
}
