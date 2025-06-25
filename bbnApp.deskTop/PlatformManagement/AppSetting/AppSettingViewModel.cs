using AutoMapper;
using Avalonia.Controls;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Controls;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.PlatformManagement.AreaCode;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace bbnApp.deskTop.PlatformManagement.AppSetting
{
    partial class AppSettingViewModel:BbnPageBase
    {
        public readonly IDialog dialog;
        /// <summary>
        /// 
        /// </summary>
        public readonly ISukiDialogManager dialogManager;
        /// <summary>
        /// 
        /// </summary>
        private readonly PageNavigationService nav;
        /// <summary>
        /// 配置参数数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<AppSettingDto> _appSettingItemSource = new ObservableCollection<AppSettingDto>();
        /// <summary>
        /// 总条数
        /// </summary>
        [ObservableProperty] private int _totalItems;
        /// <summary>
        /// 当前页序号
        /// </summary>
        private int _currentPage = 1;
        /// <summary>
        /// 每页条数
        /// </summary>
        private int _itemsPrePage = 20;
        /// <summary>
        /// 查询状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 选中的系统配置
        /// </summary>
        [ObservableProperty] private AppSettingDto _appSettingSelected;
        /// <summary>
        /// 查询条件
        /// </summary>
        [ObservableProperty] private string _filterSettingName = string.Empty;
        [ObservableProperty] private string _filterSettingCode = string.Empty;
        [ObservableProperty] private string _filterSettingDesc = string.Empty;
        /// <summary>
        /// 列表数据
        /// </summary>
        [ObservableProperty] private IEnumerable<AppSettingDto> _appSettingData;
        [ObservableProperty] private IEnumerable<AppSettingDto> _pagerData;
        /// <summary>
        /// 是否编辑状态
        /// </summary>
        [ObservableProperty] private bool _isEdit = false;

        /// <summary>
        /// 
        /// </summary>
        private AppSettingGrpc.AppSettingGrpcClient _client;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private UserControl _appSettingContent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public AppSettingViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("PlatformManagement", "系统配置", MaterialIconKind.Settings, "", 1)
        {
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _client = grpcClientFactory.CreateClient<AppSettingGrpc.AppSettingGrpcClient>().GetAwaiter().GetResult();
            _mapper = mapper;
            this.dialog = dialog;
        }
        /// <summary>
        /// 配置信息查询
        /// </summary>
        [RelayCommand]
        private async Task AppSettingLoad()
        {
            IsBusy = true;
            AppSettingLoadGrid();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        private async Task AppSettingLoadGrid()
        {

            dialog.ShowLoading("数据查询中", async (e) =>
            {
                AppSettingSearchRequest _Request = new AppSettingSearchRequest
                {
                    PageIndex = _currentPage,
                    PageSize = _itemsPrePage,
                    SettingCode = FilterSettingCode,
                    SettingName = FilterSettingName,
                    SettingDesc = FilterSettingDesc,
                };
                // 创建 Metadata（Header）
                var headers = CommAction.GetHeader();
                var response = await _client.AppSettingGridLoadAsync(_Request, headers);
                dialog.LoadingClose(e);
                if (response.Code)
                {
                    var _data = _mapper.Map<IEnumerable<AppSettingDto>>(response.Items);
                    AppSettingData = [.. _data];
                    TotalItems = response.Total;
                }
                else
                {
                    dialog.Error("错误提示", $"数据查询异常：{response.Message}");
                }
                IsBusy = false;
            });
            
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private void SettingEdit(object? obj)
        {
            if (obj is not AppSettingDto model) return;
            AppSettingEdit(model);
        }

        /// <summary>
        /// 新建
        /// </summary>
        [RelayCommand]
        private void SettingAddAction()
        {
            AppSettingDto Item = new AppSettingDto
            {
                SettingId = string.Empty,
                SettingCode = string.Empty,
                SettingName = string.Empty,
                SettingDesc = string.Empty,
                SettingType = "string",
                SettingIndex=int.MinValue,
                ValueRange = string.Empty,
                DefaultValue = string.Empty,
                NowValue = string.Empty,
                ReMarks = string.Empty,
                IsFiexed = 0,
                IsLock = 0,
                LockReason = string.Empty,
                LockTime = string.Empty
            };
            AppSettingEdit(Item);
        }
        /// <summary>
        /// 删除
        /// </summary>
        [RelayCommand]
        private async Task SettingDelete(object? obj)
        {
            if (obj is not AppSettingDto item) return;
            var b = await dialog.Confirm("删除提示","确定要删除当前选中的配置信息吗?","删除","取消");
            if (b)
            {
                await SettingState("IsDelete",item);
            }
        }
        /// <summary>
        /// 动作
        /// </summary>
        public async Task SettingState(string type, AppSettingDto item)
        {
            dialog.ShowLoading("数据提交中", async (e) =>
            {
                string tips = string.Empty;
                if (string.IsNullOrEmpty(type))
                {
                    _ = dialog.Alert( "提示", "操作类型不能为空！", "确定");
                }
                else
                {
                    var itemdata = _mapper.Map<Protos.AppSetting>(item);
                    AppSettingStateRequest settingStateRequest = new AppSettingStateRequest
                    {
                        Type = type,
                        Item = itemdata,
                    };
                    var headers = CommAction.GetHeader();
                    var response = await _client.AppSettingStateSaveAsync(settingStateRequest, headers);
                    dialog.LoadingClose(e);
                    if (response.Code)
                    {
                        dialog.Success("提示", response.Message);
                        await AppSettingLoadGrid();
                    }
                    else
                    {
                        dialog.Error("错误提示", $"数据处理异常：{response.Message}");
                    }
                }
            });
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="pagedData"></param>
        public void OnPageChanged(Pagination pagination, int currentPage, int itemsPerPage, IEnumerable pagedData)
        {
            try
            {
                _itemsPrePage = itemsPerPage;
                _currentPage = currentPage;

                bool isBackendPaging = pagination.IsBackendPaging;
                if (!isBackendPaging)
                {
                    // 伪分页模式：处理当前页的数据集
                    var pagedList = pagedData.Cast<AppSettingDto>().ToList();
                    AppSettingData = new ObservableCollection<AppSettingDto>(pagedList);
                }
                else
                {
                    // 后台分页模式：根据 currentPage 和 itemsPerPage 加载数据
                    AppSettingLoadCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("异常提示", $"分页数据加载异常：{ex.Message.ToString()}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void AppSettingEdit(AppSettingDto Item)
        {
            try
            {
                IsEdit = true;
                var viewModel = new AppSettingEditViewModel();

                AppSettingContent = new AppSettingEditView { DataContext = viewModel };
                viewModel.ViewModelInit(dialog,AppSettingSubmitCallBack, Item, _client,_mapper);

            }
            catch (Exception ex)
            {
                IsEdit = false;
                dialog.Error("异常提示", $"数据新增操作异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 代码提交回调
        /// </summary>
        private void AppSettingSubmitCallBack(bool success, string message, object data)
        {
            if (success)
            {
                dialog.Success("提示", message);
                IsEdit = false;
                AppSettingContent = null;
                _=AppSettingLoadGrid();
            }
            else if (!success && message == "关闭")
            {
                IsEdit = false;
                AppSettingContent = null;
            }
        }
    }
}
