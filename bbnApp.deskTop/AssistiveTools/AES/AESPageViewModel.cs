using Avalonia.Controls.Notifications;
using bbnApp.deskTop.Common;

using bbnApp.deskTop.Services;
using bbnApp.Share;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Linq;

namespace bbnApp.deskTop.AssistiveTools.AES
{
    public partial class AESPageViewModel : BbnPageBase
    {
        private readonly IDialog dialog;
        private readonly ISukiDialogManager dialogManager;
        private readonly PageNavigationService nav;
        /// <summary>
        /// SM2加解密
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public AESPageViewModel(IDialog dialog, PageNavigationService nav) : base("AssistiveToolsKeys", "AES加解密", MaterialIconKind.Decrement, "bbn-aes", 2)
        {
            this.dialog = dialog;
            this.nav = nav;
        }
        /// <summary>
        /// 待加解密字符串
        /// </summary>
        [ObservableProperty] private string _plaintext;
        /// <summary>
        /// 加密或解密后的字符串
        /// </summary>
        [ObservableProperty] private string _ciphertext;

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
                    dialog.Tips("提示", "请在左边输入框写入需要加密的字符串!");
                }
                else
                {
                    Ciphertext = EncodeAndDecode.AesEncrypt(Plaintext);
                    dialog.Success("加密提示", "数据加密成功!");
                }
            }
            catch (Exception ex)
            {
                dialog.Success("异常提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 数据解密
        /// </summary>
        [RelayCommand]
        private void DataDecrypt()
        {
            try
            {
                if (string.IsNullOrEmpty(_plaintext))
                {
                    dialog.Tips("提示", "请在左边输入框写入需要解密的字符串!");
                }
                else
                {
                    Ciphertext = EncodeAndDecode.AesDecrypt(Plaintext);
                    dialog.Success("解密提示", "数据解密成功!");
                }
            }
            catch (Exception ex)
            {
                dialog.Success("异常提示", ex.Message.ToString());
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
            dialog.Success("提示", "数据清除完成!");
        }
    }
}
