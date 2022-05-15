using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorApp2.Pages
{
    public sealed partial class LogTableView
    {
        [Inject]
        public IJSRuntime jSRuntime { get; set; }
    }
}
