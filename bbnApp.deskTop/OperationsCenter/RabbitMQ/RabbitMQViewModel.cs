using Avalonia.Controls;
using bbnApp.deskTop.Common;

using bbnApp.deskTop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SukiUI.Dialogs;
using System;
using System.Text;
using WebViewControl;

namespace bbnApp.deskTop.OperationsCenter.RabbitMQ
{
    partial class RabbitMQViewModel : BbnPageBase
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly PageNavigationService nav;
        /// <summary>
        /// 当前控件
        /// </summary>
        private UserControl NowControl;
        /// <summary>
        /// 
        /// </summary>
        public readonly IDialog dialog;
        /// <summary>
        /// 
        /// </summary>
        public readonly ISukiDialogManager dialogManager;
        /// <summary>
        /// 地址
        /// </summary>
        [ObservableProperty] private string _url = string.Empty;
        [ObservableProperty] private bool _isBusy = true;
        [ObservableProperty] private bool _isInited = false;
        /// <summary>
        /// 配置信息
        /// </summary>
        public JObject ObjSetting = new JObject();
        /// <summary>
        /// 
        /// </summary>
        private WebView webview;
        /// <summary>
        /// 
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public RabbitMQViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IDialog dialog, IConfiguration configuration) : base("OperationsCenter", "RabbitMQ平台", MaterialIconKind.Rabbit, "", 12)
        {
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            this.configuration = configuration;
        }

        public void Init(WebView webview, UserControl NowControl)
        {
            JObject Obj = JObject.Parse(Share.EncodeAndDecode.MixDecrypt(configuration["ExtApp"]?.ToString() ?? string.Empty));
            ObjSetting = JObject.Parse(Obj["rabbitmq"].ToString());
            Url = ObjSetting["url"]?.ToString() ?? string.Empty;
            this.webview = webview;
            this.NowControl = NowControl;
            this.webview.Navigated += (s, e) => {
                IsBusy = false;
                // 页面加载完成后执行 JS 脚本,自动登录
                StringBuilder script = new StringBuilder();
                script.AppendLine("const interval = setInterval(() => {");
                script.AppendLine($"const input = document.querySelector('input[name = \"{ObjSetting["usernameinput"].ToString()}\"]');");
                script.AppendLine("if (input)");
                script.AppendLine("{");
                script.AppendLine("clearInterval(interval);");

                script.Append($" document.querySelector('input[name=\"{ObjSetting["usernameinput"].ToString()}\"]').value = '{ObjSetting["username"].ToString()}';");
                script.Append($" document.querySelector('input[name=\"{ObjSetting["passwordinput"].ToString()}\"]').value = '{ObjSetting["password"].ToString()}';");
                script.Append($" document.querySelector('input[type=\"submit\"]').click();");

                script.AppendLine("}");
                script.AppendLine("}, 1000);");

                this.webview.EvaluateScript<object>(script.ToString(), null, TimeSpan.FromMilliseconds(3000));
            };

        }
        /// <summary>
        /// 刷新
        /// </summary>
        [RelayCommand]
        private void RefreshCommand()
        {

        }
    }
}
