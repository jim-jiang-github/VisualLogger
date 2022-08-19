using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using MvvmBlazor;
using MvvmBlazor.ViewModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using VisualLogger.Messages;
using VisualLogger.Utils;
using VisualLogger.Viewer.Web.Interfaces;

namespace VisualLogger.Viewer.Web.ViewModels
{
    public partial class ScenarioOptionsViewModel : DialogViewModel
    {
        [Notify]
        private bool _isLoading = false;
        [Notify]
        private string _repo = string.Empty;
        [Notify]
        private bool _isShowBranchList = false;

        private CancellationTokenSource? _cancellationTokenSource;


        public ScenarioOptionsViewModel()
        {
            IsOpen = true;
        }

        public async Task FetchBranches()
        {
            _isLoading = true;
            _cancellationTokenSource = new CancellationTokenSource();
            if (OperatingSystem.IsBrowser())
            {
                await Task.Delay(1000);
                IsShowBranchList = true;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Can not support command in blazor!");
                stringBuilder.AppendLine("We only mock 1000 ms delay here!");
                Notification.Error(stringBuilder.ToString());
            }
            else
            {
                var branches = await GitRunner.GetAllOriginBranches(Repo, true, _cancellationTokenSource.Token);
            }
            _isLoading = false;
        }

        public void CancleFetchBranches()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            _isLoading = false;
        }

        public override void OnIsOpenChanged(bool isOpen)
        {
            base.OnIsOpenChanged(isOpen);
            CancleFetchBranches();
        }
    }
}
