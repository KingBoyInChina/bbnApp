using AutoMapper;
using Avalonia.Controls;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.Share;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Org.BouncyCastle.Asn1.Ocsp;
using SukiUI.Dialogs;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.PlatformManagement.OperationCode
{
    partial class OperationCodeViewModel:BbnPageBase
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
        private OperationObjectCodeGrpc.OperationObjectCodeGrpcClient _client;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private UserControl _operationContent;
        /// <summary>
        /// 对象代码树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<OperationObjectNodeDto> _operationTreeSource = new ObservableCollection<OperationObjectNodeDto>();
        /// <summary>
        /// 选中的树节点
        /// </summary>
        [ObservableProperty] private OperationObjectNodeDto _selectedTreeNode;
        /// <summary>
        /// 选中的对象
        /// </summary>
        [ObservableProperty] private OperationObjectCodeDto _optSelected;
        /// <summary>
        /// 操作代码清单
        /// </summary>
        [ObservableProperty] private List<ObjectOperationTypeDto> _itemsData;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public OperationCodeViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("PlatformManagement", "操作对象", MaterialIconKind.UnidentifiedFlyingObject, "", 99)
        {
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _client = grpcClientFactory.CreateClient<OperationObjectCodeGrpc.OperationObjectCodeGrpcClient>();
            _mapper = mapper;
            this.dialog = dialog;
        }

        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        private void TreeInit()
        {
            try
            {
                TreeLoad();
            }
            catch(Exception ex)
            {
                dialog.Error("错误提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 树节点加载
        /// </summary>
        /// <returns></returns>
        private void TreeLoad()
        {
            try
            {
                dialog.ShowLoading("标准代码树加载中...",async (e) =>
                {
                    OperationObjectTreeRequestDto requet = new OperationObjectTreeRequestDto();

                    var data = await _client.OperationObjectTreeAsync(_mapper.Map<OperationObjectTreeRequest>(requet), CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        var list = _mapper.Map<List<OperationObjectNodeDto>>(data.Item);
                        OperationTreeSource = [.. list];
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }
                });
                
            }
            catch(Exception ex)
            {
                dialog.Error("错误提示", $"TreeLoad异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 选中的树形节点
        /// </summary>
        /// <param name="item"></param>
        public void NodeSelected(OperationObjectNodeDto item)
        {
            if (item.IsLeaf)
            {
                SelectedTreeNode = item;
                //获取明细信息
                NodeInfoLoad(item.Id);
            }
        }
        /// <summary>
        /// 读取节点的明细信息
        /// </summary>
        private void NodeInfoLoad(string ObjCode)
        {
            try
            {
                dialog.ShowLoading("节点明细信息读取中...", async (e) =>
                {
                    GetOperationInfoRequestDto request = new GetOperationInfoRequestDto { ObjCode = ObjCode };
                    var data = await _client.GetOperationInfoAsync(_mapper.Map<GetOperationInfoRequest>(request), CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        OptSelected = _mapper.Map<OperationObjectCodeDto>(data.Obj);
                        ItemsData = _mapper.Map<List<ObjectOperationTypeDto>>(data.Item);
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }
                });
            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 操作代码明细信息重载
        /// </summary>
        [RelayCommand]
        private void ListReload()
        {
            if (SelectedTreeNode != null)
            {
                NodeInfoLoad(SelectedTreeNode.Id);
            }
        }
        /// <summary>
        /// 新建
        /// </summary>
        [RelayCommand]
        private async Task ObjAdd()
        {
            OptSelected = new OperationObjectCodeDto();
            SelectedTreeNode = new OperationObjectNodeDto();
            ItemsData=new List<ObjectOperationTypeDto>();
            await Task.Delay(500);
            OptSelected = new OperationObjectCodeDto
            {
                Yhid=UserContext.CurrentUser.Yhid,
                ObjCode = "",
                IdxNum = 1,
                ObjName="",
                ObjDescription="",
                IsLock = 0,
                LockTime = DateTime.MinValue.ToString(),
                LockReason="",
                ReMarks=""
            };
        }
        /// <summary>
        /// 对象信息保存
        /// </summary>
        [RelayCommand]
        private void ObjSave()
        {
            try
            {
                IsBusy = true;
                string msg = string.Empty;
                if (string.IsNullOrEmpty(OptSelected.ObjCode))
                {
                    msg = "对象代码不能为空";
                }
                else if (string.IsNullOrEmpty(OptSelected.ObjName))
                {
                    msg = "对象名称不能为空";
                }
                else if (string.IsNullOrEmpty(OptSelected.ObjDescription))
                {
                    msg = "对象说明不能为空";
                }
                if (string.IsNullOrEmpty(msg))
                {
                    dialog.ShowLoading("数据提交中...", async (e) =>
                    {
                        SaveOperationInfoRequestDto request = new SaveOperationInfoRequestDto
                        {
                            Data = OptSelected
                        };
                        var data = await _client.SaveOperationInfoAsync(_mapper.Map<SaveOperationInfoRequest>(request), CommAction.GetHeader());
                        dialog.LoadingClose(e);
                        IsBusy = false;
                        if (data.Code)
                        {
                            OptSelected = _mapper.Map<OperationObjectCodeDto>(data.Obj);
                            dialog.Success("提示", data.Message);
                            TreeLoad();
                            NodeInfoLoad(OptSelected.ObjCode);
                        }
                        else
                        {
                            dialog.Error("提示", data.Message);
                        }
                    });
                }
                else
                {
                    dialog.Error("提示",msg);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
                IsBusy = false;
            }
        }
        /// <summary>
        /// 对象代码状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        public async Task ObjState(string type, OperationObjectNodeDto node)
        {
            if (type == "IsDelete")
            {
                bool b = await dialog.Confirm("删除提示",$"确定要删除{node.Name}吗？");
                if (b)
                {
                    ObjStatePost(type, node.Id);
                }
            }
            else if (type == "IsLock")
            {
                if (!node.IsLock)
                {
                    bool b = await dialog.Confirm("删除提示", $"确定要锁定{node.Name}吗？");
                    if (b)
                    {
                        ObjStatePost(type, node.Id);
                    }
                }
                else
                {
                    ObjStatePost(type, node.Id);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ObjCode"></param>
        private void ObjStatePost(string type,string ObjCode)
        {
            try
            {
                dialog.ShowLoading("数据提交中...",async e =>
                {
                    OperationStateRequestDto request = new OperationStateRequestDto
                    {
                        Type = type,
                        ObjCode = ObjCode
                    };
                    var data = await _client.OperationStateAsync(_mapper.Map<OperationStateRequest>(request), CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        dialog.Success("提示",data.Message);
                        OptSelected = _mapper.Map<OperationObjectCodeDto>(data.Obj);
                        //数据重载
                        TreeLoad();
                        if (type == "IsDelete")
                        {
                            SelectedTreeNode = new OperationObjectNodeDto();
                            OptSelected = new OperationObjectCodeDto();
                            ItemsData = new List<ObjectOperationTypeDto>();
                        }
                        else
                        {
                            if (SelectedTreeNode != null)
                            {
                                if (OptSelected.ObjCode == SelectedTreeNode.Id)
                                {
                                    NodeInfoLoad(OptSelected.ObjCode);
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        dialog.Error("提示",data.Message);
                    }
                });

            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 操作代码状态提交
        /// </summary>
        public void ItemSave(ObjectOperationTypeDto item)
        {
            try
            {
                if (string.IsNullOrEmpty(item.ObjCode))
                {
                    item.ObjCode = OptSelected.ObjCode;
                }
                ItemSaveRequestDto request = new ItemSaveRequestDto
                { 
                    Item=item
                };
                dialog.ShowLoading("数据提交中...",async e =>
                {
                    var data = await _client.ItemSaveAsync(_mapper.Map<ItemSaveRequest>(request),CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        dialog.Success("提示",data.Message);
                    }
                    else
                    {
                        dialog.Error("提示",data.Message);
                    }
                });
            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        
    }
}
