using Avalonia.Controls.Notifications;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Services;
using bbnApp.Share;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Splat;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;

namespace bbnApp.deskTop.AssistiveTools.AESMix
{
    public partial class AESMixPageViewModel : BbnPageBase
    {

        private readonly ISukiToastManager sukiToastManager;
        private readonly ISukiDialogManager dialogManager;
        private readonly PageNavigationService nav;
        /// <summary>
        /// "AssistiveTools", "混合加解密"
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public AESMixPageViewModel(ISukiToastManager ToastManager, ISukiDialogManager DialogManager, PageNavigationService nav) : base("AssistiveToolsKeys", "混合加解密",MaterialIconKind.Decrement, "bbn-rsaandaes",2)
        {
            this.sukiToastManager = ToastManager;
            this.dialogManager = DialogManager;
            this.nav = nav;
        }
        /// <summary>
        /// 待加解密字符串
        /// </summary>
        [ObservableProperty] private string _plaintext = null!;
        /// <summary>
        /// 加密或解密后的字符串
        /// </summary>
        [ObservableProperty] private string _ciphertext = null!;

        /// <summary>
        /// 数据加密
        /// </summary>
        [RelayCommand]
        private void DataEncrypt()
        {
            try
            {
                if (string.IsNullOrEmpty(_plaintext))
                {
                    dialogManager.CreateDialog()
                    .WithTitle("提示")
                    .WithContent("请在左边输入框写入需要加密的字符串!")
                    .Dismiss().ByClickingBackground()
                    .OfType(NotificationType.Warning)
                    .WithActionButton("确定", _ => { })
                    .TryShow();
                }
                else
                {
                    Ciphertext = EncodeAndDecode.MixEncrypt(Plaintext);
                    sukiToastManager.CreateToast()
                        .WithTitle("加密提示")
                        .WithContent("数据加密成功!")
                        .OfType(NotificationType.Success)
                        .Dismiss().ByClicking()
                        .Dismiss().After(TimeSpan.FromSeconds(3))
                        .Queue();
                }
            }
            catch(Exception ex)
            {
                dialogManager.CreateDialog()
                    .WithTitle("异常提示")
                    .WithContent(ex.Message.ToString())
                    .Dismiss().ByClickingBackground()
                    .OfType(NotificationType.Error)
                    .WithActionButton("确定", _ => { })
                    .TryShow();
            }
        }
        /// <summary>
        /// 数据解密
        /// </summary>
        [RelayCommand]
        private void DataDecrypt() {
            try
            {
                if (string.IsNullOrEmpty(_plaintext))
                {
                    dialogManager.CreateDialog()
                    .WithTitle("提示")
                    .WithContent("请在左边输入框写入需要解密的字符串!")
                    .Dismiss().ByClickingBackground()
                    .OfType(NotificationType.Warning)
                    .WithActionButton("确定", _ => { dialogManager.DismissDialog(); })
                    .TryShow();
                }
                else
                {
                    Ciphertext = EncodeAndDecode.MixDecrypt(Plaintext);
                    sukiToastManager.CreateToast()
                        .WithTitle("解密提示")
                        .WithContent("数据解密成功!")
                        .OfType(NotificationType.Success)
                        .Dismiss().ByClicking()
                        .Dismiss().After(TimeSpan.FromSeconds(3))
                        .Queue();
                }
            }
            catch (Exception ex)
            {
                dialogManager.CreateDialog()
                    .WithTitle("异常提示")
                    .WithContent(ex.Message.ToString())
                    .Dismiss().ByClickingBackground()
                    .OfType(NotificationType.Error)
                    .WithActionButton("确定", _ => { })
                    .TryShow();
            }
        }
        /// <summary>
        /// 数据清楚
        /// </summary>
        [RelayCommand]
        private void DataClear()
        {
            Plaintext = string.Empty;
            Ciphertext = string.Empty;
            sukiToastManager.CreateToast()
                        .WithTitle("提示")
                        .WithContent("数据清除完成!")
                        .OfType(NotificationType.Success)
                        .Dismiss().ByClicking()
                        .Dismiss().After(TimeSpan.FromSeconds(3))
                        .Queue();
        }
    }
}
