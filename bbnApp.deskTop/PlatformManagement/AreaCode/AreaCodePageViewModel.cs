using Avalonia.Controls;
using bbnApp.Core;
using bbnApp.deskTop.Controls;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Services;
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
using bbnApp.Common.Models;
using BbnApp.Protos;
using bbnApp.GrpcClients;
using bbnApp.deskTop.Common;
using bbnApp.DTOs.CodeDto;
using System.Threading.Tasks;
using AutoMapper;
using bbnApp.deskTop.Common.CommonViews;

namespace bbnApp.deskTop.PlatformManagement.AreaCode
{
    public partial class AreaCodePageViewModel : BbnPageBase
    { 
        /// <summary>
        /// 
        /// </summary>
        public readonly ISukiDialogManager dialogManager;
        /// <summary>
        /// 
        /// </summary>
        private readonly IDialog dialog;
        /// <summary>
        /// 
        /// </summary>
        private readonly PageNavigationService nav;
        /// <summary>
        /// 地区数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<AreaItem> _areaItemSource = new ObservableCollection<AreaItem>();
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
        /// 行政级别字典
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _xzjbItem = null!;
        /// <summary>
        /// 查询状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 查询条件-行政区划代码
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _areaCodeSelected;
        /// <summary>
        /// 默认选中项
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _initValue;
        /// <summary>
        /// 查询条件-行政区划名称
        /// </summary>
        [ObservableProperty] private string _filterAreaName = string.Empty;
        /// <summary>
        /// 查询条件-行政区划级别
        /// </summary>
        [ObservableProperty] private ComboboxItem _areaLeveSelectedItem = null!;
        /// <summary>
        /// 列表数据
        /// </summary>
        [ObservableProperty] private IEnumerable<AreaItemDto> _areaData;
        /// <summary>
        /// 是否编辑状态
        /// </summary>
        [ObservableProperty] private bool _isEdit = false;
        /// <summary>
        /// 
        /// </summary>
        private readonly IGrpcClientFactory _grpcClientFactory;

        /// <summary>
        /// 
        /// </summary>
        private AreaGrpc.AreaGrpcClient _client;
        /// <summary>
        /// 地区维护组件
        /// </summary>
        [ObservableProperty] private UserControl _areaContent;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public AreaCodePageViewModel(IDialog dialog, ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper) : base("PlatformManagement", "行政区划编码", MaterialIconKind.Location, "", int.MinValue)
        {
            this.dialog = dialog;
            this.dialogManager = DialogManager;
            this.nav = nav;
            _grpcClientFactory = grpcClientFactory;
            _client = _grpcClientFactory.CreateClient<AreaGrpc.AreaGrpcClient>();
            _mapper = mapper;
            XzjbItem = new ObservableCollection<ComboboxItem>
            {
                new ComboboxItem ("2","省","2" ),
                new ComboboxItem ("3","市","3" ),
                new ComboboxItem ("4","县","4" ),
                new ComboboxItem ("5","乡","5" ),
                new ComboboxItem ("6","村","6" )
            };
        }
        /// <summary>
        /// 数据查询
        /// </summary>
        [RelayCommand]
        private async void DataLoad()
        {
            await AreaGridLoad();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task AreaGridLoad()
        {
            IsBusy = true;

            dialog.ShowLoading("数据查询中", async (data) =>
            {
                string arecode = UserContext.CurrentUser?.AreaCode;
                AreaGridRequest _areaRequest = new AreaGridRequest
                {
                    PageIndex = _currentPage,
                    PageSize = _itemsPrePage,
                    AreaCode = AreaCodeSelected?.AreaId,
                    AreaName = FilterAreaName,
                    AreaLeve = AreaLeveSelectedItem == null ? 0 : Convert.ToInt32(AreaLeveSelectedItem.Id),
                };
                // 创建 Metadata（Header）
                var headers = CommAction.GetHeader();
                var response = await _client.GetAreaGridAsync(_areaRequest, headers);
                if (response.Code)
                {
                    var _data = _mapper.Map<IEnumerable<AreaItemDto>>(response.AreaItems);
                    AreaData = [.. _data];
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
        /// 地区代码新增动作
        /// </summary>
        [RelayCommand]
        private void AreaAddAction(object obj)
        {
            try
            {
                IsEdit = true;
                AreaItemDto areaItem = new AreaItemDto
                {
                    AreaId = string.Empty,
                    AreaPId = AreaCodeSelected.AreaId,
                    AreaName = string.Empty,
                    AreaFullName = string.Empty,
                    AreaLeve = "4",
                    AreaLeveName = "县",
                    AreaPoint = string.Empty,
                    ReMarks = string.Empty,
                    IsLock = false,
                    LockReason = string.Empty,
                    LockTime = string.Empty
                };
                var viewModel = new AreaCodeEditPageViewModel(dialog);

                AreaContent = new AreaCodeEditPage { DataContext = viewModel };
                viewModel.ViewModelInit(AreaSubmitCallBack, areaItem, InitValue, _client);

            }
            catch (Exception ex)
            {
                IsEdit = false;
                dialog.Error("异常提示", $"数据新增操作异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 地区代码提交回调
        /// </summary>
        private void AreaSubmitCallBack(bool success, string message, object data)
        {
            if (success)
            {
                dialog.Success("提示", message);
                IsEdit = false;
                AreaContent = null;
                _ = AreaGridLoad();
            }
            else if (!success && message == "关闭")
            {
                IsEdit = false;
                AreaContent = null;
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private void AreaEditAction(object obj)
        {
            try
            {
                IsEdit = true;
                AreaItemDto areaItem = (AreaItemDto)obj;
                if ((bool)!areaItem.IsLock)
                {
                    areaItem.LockTime = string.Empty;
                }
                AreaTreeNodeDto InitValue = new AreaTreeNodeDto
                {
                    AreaId = areaItem.AreaPId,
                    AreaName = areaItem.AreaFullName,
                    AreaLeve = areaItem.AreaLeve,
                    AreaLeveName = areaItem.AreaLeveName
                };
                var viewModel = new AreaCodeEditPageViewModel(dialog);
                AreaContent = new AreaCodeEditPage { DataContext = viewModel };
                viewModel.ViewModelInit(AreaSubmitCallBack, areaItem, InitValue, _client);
            }
            catch (Exception ex)
            {
                IsEdit = false;
                dialog.Error("异常提示", $"数据修改操作异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private async Task AreaDeleteAction(object obj)
        {
            AreaItemDto areaItem = (AreaItemDto)obj;
            bool b = await dialog.Confirm("删除提示", $"确定要删除{areaItem.AreaName}吗？");
            if (b)
            {
                IsEdit = true;
                dialog.ShowLoading("数据删除中", async (data) =>
                {
                    var header = CommAction.GetHeader();

                    AreaDeleteRequest request = new AreaDeleteRequest
                    {
                        AreaId = areaItem.AreaId
                    };

                    AreaDeleteResponse response = await _client.AreaDeleteAsync(request, header);
                    if (response.Code)
                    {
                        dialog.Success("删除提示", $"{response.Message}");
                        _ = AreaGridLoad();
                    }
                    else
                    {
                        dialog.Error("错误提示", $"数据删除异常：{response.Message}");
                    }
                });
            }
            IsEdit = false;

        }
        /// <summary>
        /// 停用
        /// </summary>
        /// <param name="obj"></param>
        [RelayCommand]
        private async Task AreaLockAction(object obj)
        {
            dialog.ShowLoading("数据处理中中", async (data) =>
            {
                AreaItemDto areaItem = (AreaItemDto)obj;
                if ((bool)areaItem.IsLock)
                {
                    string info = (bool)areaItem.IsLock ? $"确定要解除{areaItem.AreaName}的锁定吗？" : $"确定要锁定{areaItem.AreaName}吗？";
                    bool b = await dialog.Confirm("提示", info);
                    if (b)
                    {
                        await AreaLockSubmit(areaItem, string.Empty);
                    }
                }
                else
                {
                    dialogManager.CreateDialog()
                        .WithViewModel(dialog => new InputPromptViewModel(dialog, async (v) =>
                        {
                            if (!string.IsNullOrEmpty(v))
                            {
                                await AreaLockSubmit(areaItem, v);
                            }
                        }))
                        .TryShow();
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaItem"></param>
        /// <param name="LockReason"></param>
        /// <returns></returns>
        private async Task AreaLockSubmit(AreaItemDto areaItem, string LockReason)
        {
            var header = CommAction.GetHeader();

            AreaLockRequest request = new AreaLockRequest
            {
                AreaId = areaItem.AreaId,
                LockReason = LockReason,
            };

            AreaLockResponse response = await _client.AreaLockAsync(request, header);
            if (response.Code)
            {
                dialog.Success("提示", $"地区状态操作成功");
                _ = AreaGridLoad();
            }
            else
            {
                dialog.Error("错误提示", $"数据删除异常：{response.Message}");
            }
        }

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
                    var pagedList = pagedData.Cast<AreaItemDto>().ToList();
                    AreaData = new ObservableCollection<AreaItemDto>(pagedList);
                }
                else
                {
                    // 后台分页模式：根据 currentPage 和 itemsPerPage 加载数据
                    DataLoad();
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
    }
}
