using Avalonia.Controls.Notifications;
using bbnApp.deskTop.Common;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Exceptionless;
using Serilog;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Linq;
using Grpc.Core;

namespace bbnApp.deskTop.ViewModels
{
    public partial class ChangePassWordViewModel : ViewModelBase
    {
        private readonly Author.AuthorClient client;

        private readonly IDialog dialog;
        private readonly ISukiDialog window;
        /// <summary>
        /// 当前登录人员
        /// </summary>
        //public UserInfo User => UserContext.CurrentUser;
        /// <summary>
        /// 
        /// </summary>
        private ILogger _ILogger;
        /// <summary>
        /// 
        /// </summary>
        private ExceptionlessClient _ExceptionlessClient;
        /// <summary>
        /// 
        /// </summary>
        private Action<ISukiDialog, PassWordResponse?> _ac;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="sukiToastManager"></param>
        /// <param name="client"></param>
        /// <param name="ILogger"></param>
        /// <param name="ExceptionlessClient"></param>
        /// <param name="ac"></param>
        /// <param name="tipsinfo"></param>
        public ChangePassWordViewModel(ISukiDialog window, IDialog dialog, Author.AuthorClient client, ILogger ILogger, ExceptionlessClient ExceptionlessClient,Action<ISukiDialog,PassWordResponse?> ac,string tipsinfo) {
            this.window = window;
            this.client = client;
            this.dialog = dialog;
            this._ac = ac;
            _ILogger = ILogger;
            _ExceptionlessClient = ExceptionlessClient;
            TipsInfo = tipsinfo;
        }
        /// <summary>
        /// 提交状态
        /// </summary>
        [ObservableProperty] private string _tipsInfo;
        /// <summary>
        /// 提交状态
        /// </summary>
        [ObservableProperty] private bool _isSubmiting;
        /// <summary>
        /// 旧密码
        /// </summary>
        [ObservableProperty] private string _oldPassword;
        /// <summary>
        /// 新密码
        /// </summary>
        [ObservableProperty] private string _newPassword;
        /// <summary>
        /// 新密码复核
        /// </summary>
        [ObservableProperty] private string _newPasswordC;
        /// <summary>
        /// 密码变更提交
        /// </summary>
        [RelayCommand]
        private async void Submit()
        {
            try
            {
                string Msg = string.Empty;
                if (string.IsNullOrEmpty(OldPassword))
                {
                    Msg = "请输入旧密码";
                }
                else if (string.IsNullOrEmpty(NewPassword))
                {
                    Msg = "请输入新密码";
                }
                else if (NewPassword != NewPasswordC)
                {
                    Msg = "2次输入的新密码不一致";
                }
                else if(!bbnApp.Share.CommMethod.IsPasswordValid(NewPassword))
                {
                    Msg = "密码长度为8位，包含大小写字母、数字、特殊字符";
                }
                if (string.IsNullOrEmpty(Msg))
                {
                    if (!IsSubmiting)
                    {
                        IsSubmiting = true;
                        PassWordRequest _request = new PassWordRequest
                        {
                            OldPassWord = OldPassword,
                            NewPassWord=NewPassword
                        };
                        // 创建 Metadata（Header）
                        var headers =CommAction.GetHeader();
                        var response = await client.UpdatePassWordAsync(_request, headers);
                        IsSubmiting = false;
                        if (response.Code)
                        {
                            _ac(window,response);
                        }
                        else
                        {
                            dialog.Error("修改失败", response.Message);
                        }
                    }
                    else
                    {
                        dialog.Tips("进度提示", "正在处理中,请勿重复操作",2, NotificationType.Warning);
                    }
                }
                else
                {
                    dialog.Error("错误提示", Msg);
                }
            }
            catch (Exception ex)
            {
                _ILogger.Error($"密码修改异常：{ex.Message.ToString()}");
                _ExceptionlessClient.SubmitException(new Exception($"密码修改异常：{ex.Message.ToString()}"));
                dialog.Error("错误提示", ex.Message.ToString());
            }
            finally
            {
                IsSubmiting = false;
            }
        }
        /// <summary>
        /// 窗口关闭
        /// </summary>
        [RelayCommand]
        private void DialogClose()
        {
            window.Dismiss();
        }
    }
}
