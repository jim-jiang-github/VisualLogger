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
using VisualLogger.Extensions;
using VisualLogger.Localization;
using VisualLogger.Messages;
using VisualLogger.Scenarios;
using VisualLogger.Viewer.Data;
using VisualLogger.Viewer.Interfaces;
using VisualLogger.Viewer.Pages;
using VisualLogger.Viewer.Services;
using VisualLogger.Viewer.ViewModels;

namespace VisualLogger.Viewer.Extensions
{
    public static class VisualLoggerWebServiceCollectionExtensions
    {
        public static IServiceCollection AddVisualLoggerWeb(this IServiceCollection services)
        {
            services.AddMvvm();
            services.AddHotKeys();
            services.AddVisualLogger();
            services.AddMasaBlazor(options =>
            {
                options.ConfigureTheme(theme =>
                {
                    theme.Dark = true;
                });
            });
            services.AddI18n();
            services.AddScoped<II18nSource, I18nSource>();
            services.AddScoped<SearchService>();
            services.AddScoped<Scenario>();
            services.AddScoped<MenuBarService>();

            //services.AddScoped<MainLayoutViewModel>();
            services.AddScoped<ScenarioOptionsViewModel>();
            services.AddScoped<ModelDialogContainerViewModel>();
            services.AddScoped((provider) =>
            {
                var modelDialog = provider.GetService<ModelDialogContainerViewModel>();
                if (modelDialog == null)
                {
                    return IModelDialog.Default;
                }
                else
                {
                    return modelDialog;
                }
            });
            services.AddScoped<NotificationContainerViewModel>();
            services.AddScoped((provider) =>
            {
                var notification = provider.GetService<NotificationContainerViewModel>();
                if (notification == null)
                {
                    return INotification.Default;
                }
                else
                {
                    return notification;
                }
            });
            return services;
        }

        public static IServiceCollection AddI18n(this IServiceCollection services)
        {
            var assemblyDir = $"VisualLogger.Localization.SupportedCultures";
            var assembly = Assembly.GetAssembly(typeof(II18nSource));
            var supportCultures = assembly?
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
