using AutoMapper;
using Avalonia.Controls;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.PlatformManagement.AppSetting;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using BbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Material.Icons;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.PlatformManagement.DictionaryCode
{
    partial class DictionaryCodeViewModel:BbnPageBase
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
        /// 查询状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 是否编辑状态
        /// </summary>
        [ObservableProperty] private bool _isEdit = false;
        /// <summary>
        /// 
        /// </summary>
        private DataDictionaryGrpc.DataDictionaryGrpcClient _client;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private UserControl _dictionaryContent;
        /// <summary>
        /// 字典树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<DicTreeItemDto> _dicTreeSource = new ObservableCollection<DicTreeItemDto>();
        /// <summary>
        /// 选中的树节点
        /// </summary>
        private DicTreeItemDto _selectedTreeNode;
        /// <summary>
        /// 选中的字典
        /// </summary>
        [ObservableProperty] private DataDictionaryCodeDto _dicSelected;
        /// <summary>
        /// 查询条件
        /// </summary>
        [ObservableProperty] private string _filterName = string.Empty;
        /// <summary>
        /// 字典列表数据
        /// </summary>
        [ObservableProperty] private List<DataDictionaryItemDto> _itemData;
        /// <summary>
        /// 选中的字典
        /// </summary>
        [ObservableProperty] private DataDictionaryItemDto _itemSelected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public DictionaryCodeViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("PlatformManagement", "数据字典", MaterialIconKind.Dictionary, "", 2)
        {
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _client = grpcClientFactory.CreateClient<DataDictionaryGrpc.DataDictionaryGrpcClient>();
            _mapper = mapper;
            this.dialog = dialog;
        }

        #region 字典树初始化
        /// <summary>
        /// 树重载
        /// </summary>
        [RelayCommand]
        private void TreeLoad()
        {
            try
            {
                TreeInit();
            }
            catch(Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 字典类别树
        /// </summary>
        /// <returns></returns>
        public void TreeInit()
        {
            try
            {
                dialog.ShowLoading("字典树加载中", async (e) =>
                {
                    DataDictionaryTreeRequestDto request = new DataDictionaryTreeRequestDto { FilterKey = FilterName };
                    var header = CommAction.GetHeader();
                    var data = await _client.DicTreeAsync(_mapper.Map<DataDictionaryTreeRequest>(request), header);
                    if (data.Code)
                    {
                        dialog.Success("提示", data.Message);
                        DicTreeSource = [.. _mapper.Map<List<DicTreeItemDto>>(data.Item)];
                        dialog.LoadingClose(e);
                    }
                    else
                    {
                        dialog.Error("错误提示", data.Message);
                    }
                    dialog.LoadingClose(e);
                });
            }
            catch(Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 树选中
        /// </summary>
        /// <param name="node"></param>
        public async Task TreeSelecte(DicTreeItemDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if (node.IsLeaf)
                {
                    await NodeInfoLoad(node);
                }
            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private async Task NodeInfoLoad(DicTreeItemDto node)
        {
            DataDictionaryInfoRequestDto request = new DataDictionaryInfoRequestDto { DicCode = node.Id };
            var header = CommAction.GetHeader();
            var data = await _client.DicInfoAsync(_mapper.Map<DataDictionaryInfoRequest>(request),header);
            if (data.Code)
            {
                DicSelected = _mapper.Map<DataDictionaryCodeDto>(data.DicObj);
                ItemData = _mapper.Map<List<DataDictionaryItemDto>>(data.Items);
            }
            else
            {
                dialog.Error("提示",data.Message);
            }
        }
        /// <summary>
        /// 树节点状态变更
        /// </summary>
        public async Task NodeState(string type, DicTreeItemDto node)
        {
            if (type == "Edit")
            {
                #region 修改
                await NodeEdit(false);
                #endregion
            }
            else if (type == "IsDelete")
            {
                #region 删除
                bool b = await dialog.Confirm("删除提示",$"确定要删除字典分类{node.Name}吗？");
                if (b)
                {
                    NodeStateSubmit(type,node);
                }
                #endregion
            }
            else if (type == "IsLock")
            {
                #region 锁定
                if (node.IsLock)
                {
                    NodeStateSubmit(type, node);
                }
                else
                {
                    bool b = await dialog.Confirm("删除提示", $"确定要锁定字典分类{node.Name}吗？");
                    if (b)
                    {
                        NodeStateSubmit(type, node);
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private void NodeStateSubmit(string type, DicTreeItemDto node)
        {
            try
            {
                dialog.ShowLoading("请求处理中", async (e) => {
                    DataDictionaryStateRequestDto request = new DataDictionaryStateRequestDto
                    {
                        Item = node,
                        Type = type
                    };
                    var data = await _client.DicStateSaveAsync(_mapper.Map<DataDictionaryStateRequest>(request), CommAction.GetHeader());
                    if (data.Code)
                    {
                        dialog.Success("提示", data.Message);
                        TreeInit();//重载树
                        ItemsList();
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }
                    dialog.LoadingClose(e);
                });
            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 字典类别新增
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task NodeEdit(bool creat=true)
        {
            if (creat)
            {
                if (DicSelected != null)
                {
                    bool b = await dialog.Confirm("操作提示", $"确定要在当前选中的节点{DicSelected.DicName}下新建新节点吗？", "确定", "取消");
                    if (b)
                    {
                        NodeWindow(GetNewNode(DicSelected.DicCode), _selectedTreeNode);
                    }
                    else
                    {
                        NodeWindow(GetNewNode(), new DicTreeItemDto { Name = "根节点", Id = "-1" });
                    }
                }
                else
                {
                    NodeWindow(GetNewNode(), new DicTreeItemDto { Name = "根节点", Id = "-1" });
                }
            }
            else if(DicSelected!=null) {
                if (DicSelected.IsLock == 1)
                {
                    dialog.Tips("提示","已锁定的数据不能修改");
                }
                else
                {
                    DicTreeItemDto PItem = FindParentNode([..DicTreeSource],DicSelected.DicPCode);
                    NodeWindow(DicSelected, new DicTreeItemDto { Name = PItem.Name, Id = PItem.Id });
                }
            }
        }
        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="root"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public DicTreeItemDto FindParentNode(List<DicTreeItemDto> nodes, string targetId)
        {
            // 如果根节点为空，直接返回 null
            if (nodes == null)
                return null;

            // 遍历根节点的子节点
            foreach (var child in nodes)
            {
                // 如果子节点的 ID 等于目标 ID，则根节点就是父节点
                if (child.Id == targetId)
                    return child;

                // 递归查找子节点的子树
                var parent = FindParentNode(child.SubItems, targetId);
                if (parent != null)
                    return parent;
            }

            // 如果未找到，返回 null
            return null;
        }
        /// <summary>
        /// 新增节点初始对象
        /// </summary>
        /// <param name="DicPCode"></param>
        /// <returns></returns>
        private DataDictionaryCodeDto GetNewNode(string DicPCode="-1")
        {
            return new DataDictionaryCodeDto {
                Yhid = "000000",
                DicCode = "",
                DicPCode = DicPCode,
                DicIndex = int.MinValue,
                DicSpell="",
                AppId = "",
                AppName = "",
                LockReason="",
                LockTime=""
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="treenode"></param>
        private void NodeWindow(DataDictionaryCodeDto data,DicTreeItemDto node)
        {
            try
            {
                IsEdit = true;
                var viewModel = new DictionaryCodeEditViewModel(dialog);

                DictionaryContent = new DictionaryCodeEditView { DataContext = viewModel };
                viewModel.ViewModelInit(DicNodeCallBack, data,node, _client,_mapper);
            }
            catch (Exception ex)
            {
                IsEdit = false;
                dialog.Error("异常提示", $"数据新增操作异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 树节点维护回调
        /// </summary>
        /// <param name="b"></param>
        /// <param name="data"></param>
        private void DicNodeCallBack(bool b,string msg,object? data)
        {
            IsEdit = false;
            DictionaryContent = null;
            if (b)
            {
                dialog.Success("提示",msg);
                TreeInit();
                ItemsLoad();
            }
        }
        #endregion
        #region 字典项目
        /// <summary>
        /// 字典项目读取
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private void ItemsLoad()
        {
            try
            {
                ItemsList();
            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 字典清单读取
        /// </summary>
        private void ItemsList()
        {
            try
            {
                dialog.ShowLoading("字典清单加载中", async (e) =>
                {
                    DataDictionaryItemSearchRequestDto request = new DataDictionaryItemSearchRequestDto { 
                        DicCode=DicSelected.DicCode,
                        ItemName=""
                    };
                    var header = CommAction.GetHeader();
                    var data = await _client.ItemsSearchAsync(_mapper.Map<DataDictionaryItemSearchRequest>(request), header);
                    if (data.Code)
                    {
                        dialog.Success("提示", data.Message);
                        ItemData = [.. _mapper.Map<List<DataDictionaryItemDto>>(data.Items)];
                    }
                    else
                    {
                        dialog.Error("错误提示", data.Message);
                    }
                    dialog.LoadingClose(e);
                });
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 字典项目状变更
        /// </summary>
        public async Task ItemState(string type,DataDictionaryItemDto item)
        {
            try
            {
                if (type == "IsDelete")
                {
                    #region 删除
                    bool b = await dialog.Confirm("删除提示", $"确定要删除字典项目{item.ItemName}吗？");
                    if (b)
                    {
                        ItemStateSubmit(type, item);
                    }
                    #endregion
                }
                else if (type == "IsLock")
                {
                    #region 锁定
                    if (item.IsLock == 1)
                    {
                        bool b = await dialog.Confirm("删除提示", $"确定要锁定字典项目{item.ItemName}吗？");
                        if (b)
                        {
                            ItemStateSubmit(type, item);
                        }
                    }
                    else
                    {
                        ItemStateSubmit(type, item);
                    }
                    #endregion
                }

            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 项目状态变更提交
        /// </summary>
        /// <param name="type"></param>
        /// <param name="item"></param>
        private void ItemStateSubmit(string type, DataDictionaryItemDto item)
        {
            try
            {
                dialog.ShowLoading("请求处理中", async (e) => {
                    DataDictionaryItemStateRequestDto request = new DataDictionaryItemStateRequestDto
                    {
                        ItemId = item.ItemId,
                        Type = type
                    };
                    var data = await _client.ItemStateSaveAsync(_mapper.Map<DataDictionaryItemStateRequest>(request), CommAction.GetHeader());
                    if (data.Code)
                    {
                        dialog.Success("提示", data.Message);
                        ItemsList();
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }
                    dialog.LoadingClose(e);
                });
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 新建字典项目
        /// </summary>
        [RelayCommand]
        private void ItemAdd()
        {
            try
            {
                if (DicSelected != null)
                {
                    DataDictionaryItemDto item = GetNewItem();
                    ItemWindow(item);
                }
                else
                {
                    dialog.Tips("提示","请先选择字典项目所属类目");
                }
            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void ItemWindow(DataDictionaryItemDto model)
        {
            try
            {
                IsEdit = true;
                var viewModel = new DictionaryItemViewModel(dialog);

                DictionaryContent = new DictionaryItemView { DataContext = viewModel };
                viewModel.ViewModelInit(DicItemCallBack, DicSelected, model, _client, _mapper);
            }
            catch (Exception ex)
            {
                IsEdit = false;
                dialog.Error("异常提示", $"数据新增操作异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 新增项目初始对象
        /// </summary>
        /// <param name="DicPCode"></param>
        /// <returns></returns>
        private DataDictionaryItemDto GetNewItem()
        {
            return new DataDictionaryItemDto
            {
                Yhid = DicSelected.Yhid,
                DicCode = DicSelected.DicCode,
                ItemIndex = ItemData==null?1:ItemData.Count+1,
                ItemSpell = "",
                ItemId = "",
                ItemName= "",
                IsLock = 0,
                LockReason ="",
                LockTime="",
                ReMarks=""
            };
        }
        /// <summary>
        /// 项目维护回调
        /// </summary>
        /// <param name="b"></param>
        /// <param name="data"></param>
        private void DicItemCallBack(bool b, string msg, object? data)
        {
            IsEdit = false;
            DictionaryContent = null;
            if (b)
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    dialog.Success("提示", msg);
                }
                ItemsList();
            }
        }
        #endregion

    }
}
