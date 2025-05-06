using bbnApp.deskTop.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.Common.CommonViews
{
    partial class InputPromptViewModel : ViewModelBase
    {
        private readonly ISukiDialog _sukiDialog;
        private readonly Action<string> _callback;
        public InputPromptViewModel(ISukiDialog dialog,Action<string> callback)
        {
            _sukiDialog = dialog;
            _callback = callback;
        }
        /// <summary>
        /// 输入内容
        /// </summary>
        [ObservableProperty] private string _info = string.Empty;

        [RelayCommand]
        private void Close()
        {
            _callback(Info);
            _sukiDialog.Dismiss();
        }
    }
}
