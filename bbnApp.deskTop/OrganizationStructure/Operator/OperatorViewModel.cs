using AutoMapper;
using Avalonia.Controls;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.Protos;
using bbnApp.Share;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Net.Client.Balancer;
using Material.Icons;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.OrganizationStructure.Operator
{
    partial class OperatorViewModel : BbnPageBase
    {
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
        /// 
        /// </summary>
        private readonly PageNavigationService nav;
        /// <summary>
        /// 人员代码服务
        /// </summary>
        private OperatorGrpc.OperatorGrpcClient _client;
        /// <summary>
        /// 部门代码服务
        /// </summary>
        private DepartMentGrpc.DepartMentGrpcClient _departMentClient;
        /// <summary>
        /// 公司代码服务
        /// </summary>
        private CompanyGrpcService.CompanyGrpcServiceClient _companyClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 忙碌状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 操作员树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<OperatorItemDto> _operatorListSource = new ObservableCollection<OperatorItemDto>();
        /// <summary>
        /// 操作员角色数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<OperatorRoleDto> _operatorRoleListSource = new ObservableCollection<OperatorRoleDto>();
        /// <summary>
        /// 选中的人员
        /// </summary>
        [ObservableProperty] private OperatorItemDto _operatorSelected = new OperatorItemDto();
        /// <summary>
        /// 部门树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<DepartMentTreeItemDto> _departMentTreeSource = new ObservableCollection<DepartMentTreeItemDto>();
        /// <summary>
        /// 选中的部门
        /// </summary>
        [ObservableProperty] private DepartMentTreeItemDto _departMentSelected = new DepartMentTreeItemDto();
        /// <summary>
        /// 公司清单-数据集
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _companyListSource = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的公司代码
        /// </summary>
        [ObservableProperty] private ComboboxItem _companySelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 公司选项变更
        /// </summary>
        /// <param name="value"></param>
        partial void OnCompanySelectedChanged(ComboboxItem value)
        {
            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Id))
                {
                    dialog.ShowLoading("数据初始化中...", async e =>
                    {
                        try
                        {
                            //公司部门重载
                            DepartMentTreeLoad();
                            await Task.Delay(200);
                            OperatorSelected = new OperatorItemDto();
                        }
                        catch (Exception ex)
                        {
                            dialog.Error("提示", ex.Message.ToString());
                        }
                        finally
                        {
                            dialog.LoadingClose(e);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 1016 角色级别
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _leveTypeList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的角色级别
        /// </summary>
        [ObservableProperty] private ComboboxItem _leveSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 密码复核密码
        /// </summary>
        [ObservableProperty] private string _passWord = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public OperatorViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("OrganizationStructure", "权限分配", MaterialIconKind.AccessAlarms, "", 5)
        {
            _ = ClientInit(grpcClientFactory);
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;

            _mapper = mapper;
            this.dialog = dialog;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grpcClientFactory"></param>
        /// <returns></returns>
        private async Task ClientInit(IGrpcClientFactory grpcClientFactory)
        {
            _client = await grpcClientFactory.CreateClient<OperatorGrpc.OperatorGrpcClient>();
            _departMentClient = await grpcClientFactory.CreateClient<DepartMentGrpc.DepartMentGrpcClient>();
            _companyClient = await grpcClientFactory.CreateClient<CompanyGrpcService.CompanyGrpcServiceClient>();
        }

        /// <summary>
        /// 初始化字典
        /// </summary>
        public async Task OperatorDicInit(UserControl uc)
        {
            NowControl = uc;//当前控件
            await GetCompanyItems();
            CompanySelected = CommAction.SetSelectedItem(CompanyListSource, UserContext.CurrentUser.CompanyId);
            LeveTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1016"));
        }
        /// <summary>
        /// 公司清单加载
        /// </summary>
        private async Task GetCompanyItems()
        {
            try
            {
                CompanyRequestDto request = new CompanyRequestDto
                {
                    Version = string.Empty,
                    CompanyId = string.Empty,
                    Yhid = UserContext.CurrentUser.Yhid
                };
                var data = await _companyClient.GetCompanyItemsAsync(_mapper.Map<CompanyRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    var list = _mapper.Map<List<CompanyItemDto>>(data.CompanyItems);
                    var items = list.Select(j => new ComboboxItem
                    (
                        CommMethod.GetValueOrDefault(j.Id, string.Empty),
                        CommMethod.GetValueOrDefault(j.Name, string.Empty),
                        CommMethod.GetValueOrDefault(j.Tag, string.Empty)
                    )).ToList();
                    CompanyListSource = new ObservableCollection<ComboboxItem>(items);
                }
                else
                {
                    dialog.Error("提示", data.Message);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 选中公司部门树加载
        /// </summary>
        private void DepartMentTreeLoad()
        {
            dialog.ShowLoading("加载部门中...", async e =>
            {
                try
                {
                    DepartMentTreeRequestDto request = new DepartMentTreeRequestDto
                    {
                        CompanyId = CompanySelected.Id
                    };
                    var data = await _departMentClient.GetDepartMentTreeAsync(_mapper.Map<DepartMentTreeRequest>(request), CommAction.GetHeader());
                    if (data.Code)
                    {
                        DepartMentTreeSource = new ObservableCollection<DepartMentTreeItemDto>(_mapper.Map<List<DepartMentTreeItemDto>>(data.Items));
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }

                }
                catch (Exception ex)
                {
                    dialog.Error("加载部门树失败", ex.Message);
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
        /// <summary>
        /// 部门树刷新
        /// </summary>
        [RelayCommand]
        private void DepartMentTreeReload()
        {
            DepartMentTreeLoad();
        }
        /// <summary>
        /// 部门树选中
        /// </summary>
        /// <param name="node"></param>
        public void DepartMentTreeSelecte(DepartMentTreeItemDto node)
        {
            try
            {
                if (DepartMentSelected == null || DepartMentSelected?.Id != node.Id)
                {
                    DepartMentSelected = node;
                    if ((bool)node.IsLeaf)
                    {
                        {
                            OperatorListLoad();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 人员树重载
        /// </summary>
        [RelayCommand]
        private void OperatorListReload()
        {
            OperatorListLoad();
        }
        /// <summary>
        /// 加载指定部门的员工信息
        /// </summary>
        private void OperatorListLoad()
        {
            dialog.ShowLoading("人员树加载中...", async e =>
            {

                try
                {
                    OperatorSelected = new OperatorItemDto();
                    OperatorRoleListSource.Clear();
                    OperatorListSource.Clear();
                    OperatorListRequestDto request = new OperatorListRequestDto
                    {
                        DepartMentId = DepartMentSelected.Id,
                        CompanyId = DepartMentSelected.Tag,
                        EmployeeName = string.Empty,
                        EmployeeNum=string.Empty
                    };
                    var data = await _client.OperatorListLoadAsync(_mapper.Map<OperatorListRequest>(request), CommAction.GetHeader());
                    if (data.Code)
                    {
                        OperatorListSource = new ObservableCollection<OperatorItemDto>(_mapper.Map<List<OperatorItemDto>>(data.Items));
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }
                }
                catch (Exception ex)
                {
                    dialog.Error("提示", ex.Message.ToString());
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
        /// <summary>
        /// 操作员选中
        /// </summary>
        /// <param name="node"></param>
        public void OperatorSelectedAction(OperatorItemDto node)
        {
            try
            {
                OperatorSelected = node;
                if (node != null && !string.IsNullOrEmpty(node.EmployeeId))
                {
                    //加载权限
                    OperatorRoleListSource= new ObservableCollection<OperatorRoleDto>(node.OperatorRoles);
                    PassWord = OperatorSelected.PassWord;
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 权限分配保存
        /// </summary>
        [RelayCommand]
        private void OperatorSave()
        {
            if (OperatorSelected == null)
            {
                return;
            }
            if (OperatorSelected.PassWord != PassWord)
            {
                dialog.Error("提示", "两次输入的密码不一致，请重新输入！");
                PassWord = string.Empty;
                return;
            }

            if (!OperatorRoleListSource.Any(x => x.IsChecked))
            {
                dialog.Error("提示", "至少选择一种角色！");
                return;
            }
            if (string.IsNullOrEmpty(OperatorSelected.PassWord))
            {
                dialog.Error("提示", "请输入密码！");
                return;
            }
            if (OperatorSelected.PassWord!=PassWord)
            {
                dialog.Error("提示", "两次输入的密码不一致！");
                return;
            }
            if (OperatorSelected.PassWord.Length != 8)
            {
                dialog.Error("提示", "密码长度必须为8位！");
                return;
            }

            dialog.ShowLoading("权限分配中...", async e =>
            {
                try
                {

                    OperatorSelected.OperatorRoles = OperatorRoleListSource.ToList();
                    OperatorSaveRequestDto request = new OperatorSaveRequestDto
                    {
                        Item= OperatorSelected
                    };

                    var data = await _client.OperatorSaveAsync(_mapper.Map<OperatorSaveRequest>(request), CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        dialog.Success("提示", "权限分配完成！");
                        OperatorListLoad();
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }
                }
                catch (Exception ex)
                {
                    dialog.LoadingClose(e);
                    dialog.Error("提示", ex.Message.ToString());
                }
            });

        }

        /// <summary>
        /// 操作员状态变更
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="item"></param>
        public async Task OperatorState(string Type, OperatorItemDto item)
        {
            try
            {
                if (Type == "IsDelete")
                {
                    OperatorStatePost(Type, item.OperatorId, "");
                }
                else
                {
                    if (item.IsLock==0)
                    {
                        var data = await dialog.Prompt("请输入锁定原因", "确定");
                        if (data.Item1)
                        {
                            OperatorStatePost(Type, item.OperatorId, data.Item2);
                        }
                    }
                    else
                    {
                        OperatorStatePost(Type, item.OperatorId, "");
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 操作员状态变更请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Id"></param>
        /// <param name="Reason"></param>
        private void OperatorStatePost(string type, string Id, string Reason)
        {
            dialog.ShowLoading("数据处理中...", async e =>
            {
                OperatorStateRequestDto request = new OperatorStateRequestDto
                {
                    Type = type,
                    OperatorId = Id,
                    Reason = Reason
                };
                var data = await _client.OperatorStateAsync(_mapper.Map<OperatorStateRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    dialog.Success("提示", data.Message);
                    //刷新树
                    DepartMentTreeLoad();
                    if (type == "IsDelete")
                    {
                        OperatorSelected = new OperatorItemDto();
                    }
                    else
                    {
                        OperatorSelected = _mapper.Map<OperatorItemDto>(data.Item);
                    }
                }
                else
                {
                    dialog.Error("提示", data.Message);
                }
            });
        }
    }
}
