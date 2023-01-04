using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using MvvmBlazor;
using MvvmBlazor.ViewModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using VisualLogger.Messages;
using VisualLogger.Utils;
using VisualLogger.Viewer.Interfaces;

namespace VisualLogger.Viewer.ViewModels
{
    public partial class ScenarioOptionsViewModel : DialogViewModel
    {
        [Notify]
        private bool _isRepoLoading = false;
        [Notify]
        private string _repo = string.Empty;
        [Notify]
        private bool _isShowBranchList = false;
        [Notify]
        private List<string> _branches = new();
        [Notify]
        private bool _isBranchLoading = false;
        [Notify]
        private string _currentBranch = string.Empty;

        private CancellationTokenSource? _cancellationTokenSource;


        public ScenarioOptionsViewModel()
        {
            IsOpen = true;
        }

        public async Task FetchBranches()
        {
            IsRepoLoading = true;
            _cancellationTokenSource = new CancellationTokenSource();
            if (OperatingSystem.IsBrowser())
            {
                await Task.Delay(1000);
                for (int i = 0; i < 10; i++)
                {
                    Branches.Add($"asd{i}");
                }
                IsShowBranchList = true;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Can not support command in blazor!");
                stringBuilder.AppendLine("We only mock 1000 ms delay here!");
                Notification.Error(stringBuilder.ToString());
            }
            else
            {
                Branches = (await GitRunner.GetAllOriginBranches(Repo, true, _cancellationTokenSource.Token)).ToList();
                IsShowBranchList = Branches.Count() > 0;
            }
            IsRepoLoading = false;
        }

        public void CancleFetchBranches()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            IsRepoLoading = false;
            IsShowBranchList = false;
            IsBranchLoading = false;
        }

        public async Task CloneBranch(string selectedBranch)
        {
            IsBranchLoading = true;
            await GitRunner.CloneTo(Repo, selectedBranch);
            IsBranchLoading = false;
        }

        public override void OnIsOpenChanged(bool isOpen)
        {
            base.OnIsOpenChanged(isOpen);
            CancleFetchBranches();
        }
    }
}
