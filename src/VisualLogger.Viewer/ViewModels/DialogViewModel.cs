﻿using MvvmBlazor;
using MvvmBlazor.ViewModel;

namespace VisualLogger.Viewer.ViewModels
{
    public partial class DialogViewModel : ViewModelBase
    {
        [Notify]
        private bool _isOpen = false;

        public virtual void OnIsOpenChanged(bool isOpen)
        {
            _isOpen = isOpen;
        }
    }
}
