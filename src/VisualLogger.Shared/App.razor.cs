using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace VisualLogger.Shared;

/// <summary>
/// 
/// </summary>
public sealed partial class App
{
    /// <summary>
    /// 
    /// </summary>
    [Inject]
    [NotNull]
    private IJSRuntime? JSRuntime { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstRender"></param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && (
            OperatingSystem.IsBrowser()
            || OperatingSystem.IsWindows()
            || OperatingSystem.IsMacCatalyst()
            ))
        {
            await JSRuntime.InvokeVoidAsync("$.loading");
        }
    }
}
