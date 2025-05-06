using bbnApp.deskTop.Common;
using bbnApp.Share;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.ViewModels
{
    public partial class SettingViewModel : ViewModelBase
    {

        private readonly ISukiDialog window;
        private readonly IDialog dialog;
        private readonly Action<ISukiDialog, bool> _ac;
        public SettingViewModel(ISukiDialog window, IDialog dialog, Action<ISukiDialog, bool> ac)
        {
            this.window = window;
            this.dialog = dialog;
            _ac = ac;
            _=SettingInit();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private async Task SettingInit()
        {
            if (File.Exists(Path.Combine("settings", "parameSetting.json")))
            {
                string file = await File.ReadAllTextAsync(Path.Combine("settings", "parameSetting.json"));
                if (!string.IsNullOrEmpty(file))
                {
                    string data = EncodeAndDecode.MixDecrypt(file);
                    JObject Obj = JObject.Parse(data);
                    if (Obj["mqtt"] != null)
                    {
                        JObject ObjMqtt = JObject.Parse(Obj["mqtt"].ToString());
                        MqttUrl = CommMethod.GetValueOrDefault(ObjMqtt["url"], string.Empty);
                        MqttUserName = CommMethod.GetValueOrDefault(ObjMqtt["username"], string.Empty);
                        MqttPassword = ObjMqtt["password"] == null ? string.Empty : EncodeAndDecode.MixDecrypt(ObjMqtt["password"].ToString());
                    }
                    if (Obj["exceptionless"] != null)
                    {
                        JObject ObjExceptionless = JObject.Parse(Obj["exceptionless"].ToString());
                        ExcptionlessUrl = CommMethod.GetValueOrDefault(ObjExceptionless["url"], string.Empty);
                        ExcptionlessUserName = CommMethod.GetValueOrDefault(ObjExceptionless["username"], string.Empty);
                        ExcptionlessPassword = ObjExceptionless["password"] == null ? string.Empty : EncodeAndDecode.MixDecrypt(ObjExceptionless["password"].ToString());
                    }
                    if (Obj["autotheme"] != null)
                    {
                        JObject ObjAutotheme = JObject.Parse(Obj["autotheme"].ToString());
                        AutoTheme = CommMethod.GetValueOrDefault(ObjAutotheme["use"], false);
                        LightHour = CommMethod.GetValueOrDefault(ObjAutotheme["lightHour"], 8);
                        DarkHour = CommMethod.GetValueOrDefault(ObjAutotheme["darkHour"], 19);

                    }
                }
            }
        }
        /// <summary>
        /// mqtt平台地址
        /// </summary>
        [ObservableProperty] private string _mqttUrl;
        [ObservableProperty] private string _mqttUserName;
        [ObservableProperty] private string _mqttPassword;
        /// <summary>
        /// 异常监控平台地址
        /// </summary>
        [ObservableProperty] private string _excptionlessUrl;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private string _excptionlessUserName;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private string _excptionlessPassword;
        /// <summary>
        /// 按照时间自动变更暗色和亮色主题类型
        /// </summary>
        [ObservableProperty] private bool _autoTheme;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private bool _isSubmiting=false;
        /// <summary>
        /// 亮色自动切换时间
        /// </summary>
        [ObservableProperty] private int _lightHour;
        /// <summary>
        /// 暗色自动切换时间
        /// </summary>
        [ObservableProperty] private int _darkHour;
        /// <summary>
        /// 配置参数保存-保存到配置文件
        /// </summary>
        [RelayCommand]
        private async Task SettingSave()
        {
            IsSubmiting = true;
            try
            {
                JObject ObjParame = new JObject {
                { "mqtt",new JObject{
                    { "url",MqttUrl},
                    { "username",MqttUserName},
                    { "password",EncodeAndDecode.MixEncrypt(MqttPassword)}
                    }
                },
                { "exceptionless",new JObject{
                    { "url",ExcptionlessUrl},
                    { "username",ExcptionlessUserName},
                    { "password",EncodeAndDecode.MixEncrypt(ExcptionlessPassword)}
                    }
                },
                { "autotheme",new JObject{
                    {"use",AutoTheme },
                    {"lightHour",LightHour },
                    {"darkHour",DarkHour}
                }
                }
            };
                if (!Directory.Exists(Path.Combine("settings")))
                {
                    Directory.CreateDirectory(Path.Combine("settings"));
                }
                if (!File.Exists(Path.Combine("settings", "parameSetting.json")))
                {
                    File.Create(Path.Combine("settings", "parameSetting.json")).Dispose();
                }
                string parame = EncodeAndDecode.MixEncrypt(ObjParame.ToString());
                await File.WriteAllBytesAsync(Path.Combine("settings", "parameSetting.json"), Encoding.UTF8.GetBytes(parame));
                _ac(window, true);
            }
            catch(Exception ex)
            {
                dialog.Error("异常提示", ex.Message.ToString());
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
