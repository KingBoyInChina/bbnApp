using AutoMapper;
using bbnApp.deskTop.ViewModels;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
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
        private ISukiDialog _sukiDialog;
        private Action<(bool,string)> _callback;
        [ObservableProperty] private string _tips = "";

        public InputPromptViewModel()
        {
            
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="InitValue"></param>
        /// <param name="client"></param>
        public void ViewModelInit(ISukiDialog dialog, Action<(bool,string)> callback)
        {
            _sukiDialog = dialog;
            _callback = callback;
        }
        /// <summary>
        /// 输入内容
        /// </summary>
        [ObservableProperty] private string _info = string.Empty;
        /// <summary>
        /// 提交
        /// </summary>
        [RelayCommand]
        private void Submit()
        {
            if (string.IsNullOrEmpty(Info))
            {
                Tips="请填写内容";
                return;
            }
            _callback((true,Info));
            _sukiDialog.Dismiss();
        }

        [RelayCommand]
        private void Close()
        {
            _callback((false,""));
            _sukiDialog.Dismiss();
        }
    }
}
