using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.ViewModels;
using bbnApp.DTOs.CodeDto;
using BbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.PlatformManagement.AppSetting
{
    partial class AppSettingEditViewModel : ViewModelBase
    {
        /// <summary>
        /// 值类型字典
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _valueTypeItems = null!;
        /// <summary>
        /// 选中的值类型
        /// </summary>
        [ObservableProperty] private ComboboxItem _selectedValueType;
        /// <summary>
        /// 字典集合
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _dictionaryItems = null!;
        [ObservableProperty] private ComboboxItem _selectedDictionaryItem;
        /// <summary>
        /// 状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 系统配置对象
        /// </summary>
        [ObservableProperty] private AppSettingDto _appSettingItem = new AppSettingDto();
        /// <summary>
        /// 值类型时字典
        /// </summary>
        [ObservableProperty] private bool _isDictinary = false;

        private AppSettingDto _tempAppSettingItem;
        /// <summary>
        /// 
        /// </summary>
        private readonly IDialog dialog;
        /// <summary>
        /// 
        /// </summary>
        private AppSettingGrpc.AppSettingGrpcClient _client;
        /// <summary>
        /// 
        /// </summary>
        public AppSettingEditViewModel(IDialog dialog)
        {
            this.dialog = dialog;
            ValueTypeItems = new ObservableCollection<ComboboxItem>
            {
                new ComboboxItem ("string","自定义字符","string" ),
                new ComboboxItem ("combobox","字典类型","combobox" ),
                new ComboboxItem ("number","数值类型","number" ),
                new ComboboxItem ("bool","布尔类型","bool" )
            };
        }
        /// <summary>
        /// 提交回调
        /// </summary>
        private Action<bool, string, object> _appSettingSubmitCallBack;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="Node"></param>

        public void ViewModelInit(Action<bool, string, object> CallBack, AppSettingDto InitValue, AppSettingGrpc.AppSettingGrpcClient client)
        {
            _appSettingSubmitCallBack = CallBack;
            _tempAppSettingItem = InitValue;
            _client = client;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ViewModelInit()
        {
            AppSettingItem = _tempAppSettingItem;
            IsDictinary = AppSettingItem.SettingType == "combobox" ? true : false;
            #region 初始化数据
            SelectedValueType = ValueTypeItems.Where(x => x.Id == AppSettingItem.SettingType).FirstOrDefault();
            #endregion
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private async Task AppSettingSubmit()
        {
            try
            {
                string _msg = string.Empty;
                if (string.IsNullOrEmpty(AppSettingItem.SettingName))
                {
                    _msg = "参数名称不能为空";
                }
                else if (string.IsNullOrEmpty(AppSettingItem.SettingCode))
                {
                    _msg = "参数代码不能为空";
                }
                else if (string.IsNullOrEmpty(AppSettingItem.SettingDesc))
                {
                    _msg = "参数说明不能为空";
                }
                else if (string.IsNullOrEmpty(SelectedValueType?.Tag))
                {
                    _msg = "参数值类型不能为空";
                }
                else if (string.IsNullOrEmpty(AppSettingItem.DefaultValue))
                {
                    _msg = "默认值不能为空";
                }
                else if (string.IsNullOrEmpty(AppSettingItem.NowValue))
                {
                    _msg = "当前值不能为空";
                }
                if (!string.IsNullOrEmpty(_msg))
                {
                    await dialog.Alert("必填项提示", _msg, "确定");
                }
                else
                {
                    IsBusy = true;
                    BbnApp.Protos.AppSetting data = new BbnApp.Protos.AppSetting
                    {
                        IdxNum=0,
                        SettingName = AppSettingItem.SettingName,
                        SettingCode = AppSettingItem.SettingCode,
                        SettingDesc = AppSettingItem.SettingDesc,
                        NowValue = AppSettingItem.NowValue,
                        DefaultValue = AppSettingItem.DefaultValue,
                        ValueRange = AppSettingItem.ValueRange.Replace(";","；").Replace("，",","),
                        SettingIndex = AppSettingItem.SettingIndex,
                        SettingType = SelectedValueType?.Id,
                        SettingId = AppSettingItem.SettingId,
                        IsFiexed = AppSettingItem.IsFiexed,
                        IsLock = AppSettingItem.IsLock,
                        LockReason =Share.CommMethod.GetValueOrDefault(AppSettingItem.LockReason,""),
                        LockTime = Share.CommMethod.GetValueOrDefault(AppSettingItem.LockTime, "")
                    };

                    AppSettingPostRequest request = new AppSettingPostRequest
                    {
                       Item = data,
                    };

                    var header = CommAction.GetHeader();

                    AppSettingPostResponse response = await _client.AppSettingPostSaveAsync(request, header);
                    if (response.Code)
                    {
                        _appSettingSubmitCallBack(response.Code, response.Message, response.Item);
                    }
                    else
                    {
                        dialog.Error("错误提示",$"数据提交错误：{response.Message}");
                    }
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                dialog.Error("异常提示", $"数据提交异常：{ex.Message.ToString()}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task Close()
        {
            bool b = await dialog.Confirm("关闭提示", "确定要关闭当前系统配置维护页面吗？", "关闭", "取消");
            if (b)
            {
                _appSettingSubmitCallBack(false, "关闭", null);
            }
        }
    }
}