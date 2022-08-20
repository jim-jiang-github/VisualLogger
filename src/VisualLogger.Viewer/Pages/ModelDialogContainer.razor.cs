using BlazorComponent;
using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components;
using System;
using VisualLogger.Messages;
using VisualLogger.Viewer.ViewModels;

namespace VisualLogger.Viewer.Pages
{
    public partial class ModelDialogContainer
    {
        [Inject]
        private ModelDialogContainerViewModel? ViewModel { get; set; }

        private bool _value = true;
    }
}
