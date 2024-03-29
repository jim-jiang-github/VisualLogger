﻿using BlazorComponent;
using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components;
using System;
using VisualLogger.Messages;
using VisualLogger.Viewer.ViewModels;

namespace VisualLogger.Viewer.Pages
{
    public partial class NotificationContainer
    {
        [Inject]
        private NotificationContainerViewModel? ViewModel { get; set; }

        private PToast? _toast;


        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                ViewModel?.SetCurrentToast(_toast);
            }
        }
    }
}
