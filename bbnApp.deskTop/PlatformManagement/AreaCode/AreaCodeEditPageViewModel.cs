using Avalonia.Threading;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.ViewModels;
using bbnApp.DTOs.CodeDto;
using BbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Org.BouncyCastle.Crypto.Tls;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace bbnApp.deskTop.PlatformManagement.AreaCode
{
    partial class AreaCodeEditPageViewModel: ViewModelBase
    {
        /// <summary>
        /// 行政级别字典
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _xzjbItem = null!;
        /// <summary>
        /// 查询状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 选中行政区划代码
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _areaCodeSelected;
        private AreaTreeNodeDto _tempAreaCodeSelected;
        /// <summary>
        /// 默认选中项
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _initValue;
        private AreaTreeNodeDto _tempInitValue;
        /// <summary>
        /// 选中的行政级别
        /// </summary>
        [ObservableProperty] private ComboboxItem _selectedXzjb;
        private ComboboxItem _tempSelectedXzjb;
        /// <summary>
        /// 行政区划对象
        /// </summary>
        [ObservableProperty] private AreaItemDto _areaItem = new AreaItemDto();
        private AreaItemDto _tempAreaItem;
        /// <summary>
        /// 
        /// </summary>
        private readonly IDialog dialog;
        private  AreaGrpc.AreaGrpcClient _client;
        /// <summary>
        /// 
        /// </summary>
        public AreaCodeEditPageViewModel(IDialog dialog)
        {
            this.dialog = dialog;
            XzjbItem = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1001"));
        }
        /// <summary>
        /// 提交回调
        /// </summary>
        private Action<bool, string, object> _areaSubmitCallBack;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="Node"></param>
        public void ViewModelInit(Action<bool, string, object> areaSubmitCallBack, AreaItemDto Node, AreaTreeNodeDto InitValue, AreaGrpc.AreaGrpcClient client)
        {
            _tempAreaItem = Node;
            _areaSubmitCallBack = areaSubmitCallBack;
            _tempInitValue = InitValue;
            _tempAreaCodeSelected = InitValue;
            _client = client;
        }
        /// <summary>
        /// 页面初始化完毕后，再赋值
        /// </summary>
        public void ViewModelInit()
        {
            AreaItem = _tempAreaItem;
            InitValue = _tempInitValue;
            AreaCodeSelected = _tempAreaCodeSelected;
            #region 初始化数据
            SelectedXzjb = XzjbItem.Where(x => x.Id == AreaItem.AreaLeve).FirstOrDefault();
            #endregion
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private async Task AreaSubmit()
        {
            try
            {
                string _msg = string.Empty;
                if (string.IsNullOrEmpty(AreaItem.AreaName))
                {
                    _msg = "地区名称不能为空";
                }
                else if (string.IsNullOrEmpty(AreaCodeSelected?.AreaId))
                {
                    _msg = "所属行政区划不能为空";
                }
                else if (string.IsNullOrEmpty(SelectedXzjb.Id))
                {
                    _msg = "行政级别不能为空";
                }
                if (!string.IsNullOrEmpty(_msg))
                {
                    _ = dialog.Alert("必填项提示", _msg, "确定");
                }
                else
                {
                    IsBusy = true;
                    AreaPostData areaPostData = new AreaPostData
                    {
                        AreaId = AreaItem.AreaId,
                        AreaPId = AreaCodeSelected.AreaId,
                        AreaName = AreaItem.AreaName,
                        AreaLeve = SelectedXzjb.Id,
                        AreaLeveName = SelectedXzjb.Name,
                        AreaPoint = AreaItem.AreaPoint,
                        ReMarks = AreaItem.ReMarks,
                        IsLock = AreaItem.IsLock == null ? false : (bool)AreaItem.IsLock,
                        LockReason = AreaItem.LockReason
                    };

                    AreaPostRequest request = new AreaPostRequest
                    {
                        AreaData = areaPostData
                    };

                    var header = CommAction.GetHeader();

                    AreaPostResponse response = await _client.AreaPostAsync(request, header);
                    if (response.Code)
                    {
                        _areaSubmitCallBack(response.Code, response.Message, response.AreaData);
                    }
                    else
                    {
                        dialog.Error("错误提示", response.Message);
                    }
                    IsBusy = false;
                }
            }
            catch(Exception ex)
            {
                _=dialog.Alert("异常提示",ex.Message.ToString());
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
            bool b = await dialog.Confirm("关闭提示", "确定要关闭当前地区信息维护页面吗？", "关闭", "取消");
            if (b)
            {
                _areaSubmitCallBack(false, "关闭", null);
            }
        }
    }
}
