using BlazorComponent.I18n;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using VisualLogger.Scenarios;
using VisualLogger.Viewer.Web.Data;
using VisualLogger.Viewer.Web.Interfaces;
using VisualLogger.Viewer.Web.Localization;
using VisualLogger.Viewer.Web.Pages;
using VisualLogger.Viewer.Web.Services;
using VisualLogger.Viewer.Web.Shared;
using VisualLogger.Viewer.Web.ViewModels;

namespace VisualLogger.Viewer.Web.Extensions
{
    public static class VisualLoggerWebServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualLoggerWeb(this IServiceCollection services)
        {
            services.AddMvvm();
            services.AddMasaBlazor(option =>
            {
                option.DarkTheme = true;
            });
            services.AddHotKeys();
            services.AddSingleton<SearchService>();
            services.AddSingleton<Scenario>();
            services.AddSingleton<MenuBarService>();

            //services.AddSingleton<MainLayoutViewModel>();
            services.AddSingleton<ScenarioOptionsViewModel>();
            services.AddI18n();
            return services;
        }

        public static IServiceCollection AddI18n(this IServiceCollection services)
        {
            var assemblyDir = $"{nameof(VisualLogger)}.{nameof(Viewer)}.{nameof(Web)}.{nameof(Localization)}.SupportedCultures";
            var assembly = Assembly.GetExecutingAssembly();
            var supportCultures = assembly
                .GetManifestResourceNames()
                .Where(x => x.Contains(assemblyDir))
                .Where(x => Path.GetExtension(x) == ".json")
                .Select(x =>
                {
                    using Stream? stream = assembly.GetManifestResourceStream(x);
                    if (stream == null)
                    {
                        return null;
                    }
                    using StreamReader reader = new StreamReader(stream);
                    Dictionary<string, string> map = I18nReader.Read(reader.ReadToEnd());
                    return ((string, Dictionary<string, string>)?)(Path.GetFileNameWithoutExtension(x.Replace($"{assemblyDir}.", "")), map);
                })
                .Where(x => x != null)
                .Cast<(string, Dictionary<string, string>)>()
                .ToArray();
            services.AddMasaBlazor().AddI18n(supportCultures);
            return services;
        }
    }
}
