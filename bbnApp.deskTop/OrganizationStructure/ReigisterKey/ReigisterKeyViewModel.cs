using AutoMapper;
using Avalonia.Controls;
using bbnApp.deskTop.Common;

using bbnApp.deskTop.PlatformManagement.AppSetting;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.OrganizationStructure.ReigisterKey
{
    partial class ReigisterKeyViewModel : BbnPageBase
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
        /// 密钥注册服务
        /// </summary>
        private ReigisterKeyGrpcService.ReigisterKeyGrpcServiceClient _client;
        /// <summary>
        /// 编辑窗口
        /// </summary>
        [ObservableProperty] private UserControl _registerKeyContent;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 编辑状态
        /// </summary>
        [ObservableProperty] private bool _isEdit = false;

        /// <summary>
        /// 机构序列号信息
        /// </summary>
        [ObservableProperty] private ObservableCollection<CompanyAuthorRegistrKeyItemDto> _companyList = new ObservableCollection<CompanyAuthorRegistrKeyItemDto>();
        [ObservableProperty] private ObservableCollection<CompanyAuthorRegistrKeyItemDto> _filterCompanyList = new ObservableCollection<CompanyAuthorRegistrKeyItemDto>();
        [ObservableProperty] private string _companyFilter = string.Empty;
        /// <summary>
        /// 选中的机构
        /// </summary>
        [ObservableProperty] private CompanyAuthorRegistrKeyItemDto _companySelected = new CompanyAuthorRegistrKeyItemDto();
        /// <summary>
        /// 过滤条件变更
        /// </summary>
        /// <param name="value"></param>
        partial void OnCompanyFilterChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                FilterCompanyList = CompanyList;
            }
            else
            {
                var list = CompanyList.Where(x => x.CompanyName.Contains(value)).ToList() ;
                FilterCompanyList = [..list];
            }
        }
        /// <summary>
        /// 选中的机构改变
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        partial void OnCompanySelectedChanged(CompanyAuthorRegistrKeyItemDto value)
        {
            ReginsterList.Clear();
            ReginsterList =[.. value?.RegisterKeys??new List<AuthorRegisterKeyItemDto>()];
        }
        /// <summary>
        /// 选中机构的密钥清单
        /// </summary>
        [ObservableProperty] private ObservableCollection<AuthorRegisterKeyItemDto> _reginsterList = new ObservableCollection<AuthorRegisterKeyItemDto>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        /// <param name="grpcClientFactory"></param>
        /// <param name="mapper"></param>
        /// <param name="dialog"></param>
        public ReigisterKeyViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("OrganizationStructure", "密钥申请", MaterialIconKind.Security, "", 6)
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
            _client = await grpcClientFactory.CreateClient<ReigisterKeyGrpcService.ReigisterKeyGrpcServiceClient>();
        }
        /// <summary>
        /// 机构清单重载
        /// </summary>
        [RelayCommand]
        private void CompanRegisterKeysReload()
        {
            CompanRegisterKeysLoad();
        }
        /// <summary>
        /// 
        /// </summary>
        public void CompanRegisterKeysLoad()
        {
            dialog.ShowLoading("数据读取中...",async e=>
            {
                try
                {
                    CompanyAuthorRegistrKeySearchRequestDto request = new CompanyAuthorRegistrKeySearchRequestDto
                    {
                        AreaId=""
                    };
                    var response = await _client.CompanyAuthorRegistrKeySearchAsync(_mapper.Map<CompanyAuthorRegistrKeySearchRequest>(request),CommAction.GetHeader());
                    if (response.Code)
                    {
                        CompanyList =[.. _mapper.Map<List<CompanyAuthorRegistrKeyItemDto>>(response.Items)];
                        FilterCompanyList = CompanyList;
                    }
                    else
                    {
                        dialog.Error("提示",response.Message);
                    }
                }
                catch(Exception ex)
                {
                    dialog.Error("异常",ex.Message.ToString());
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
        /// <summary>
        /// 新建
        /// </summary>
        [RelayCommand]
        private void RegisterKeysAdd()
        {
            if (string.IsNullOrEmpty(CompanySelected.CompanyId))
            {
                dialog.Tips("提示", "请先选中需要申请密钥的机构");
            }
            else
            {
                var data = new AuthorRegisterKeyItemDto
                {
                    CompanyId = string.IsNullOrEmpty(CompanySelected.CompanyId) ? UserContext.CurrentUser.CompanyId : CompanySelected.CompanyId,
                    IsLock = 0
                };
                RegisterKeysAddContent(data);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void RegisterKeysAddContent(AuthorRegisterKeyItemDto Item)
        {
            try
            {
                IsEdit = true;
                var viewModel = new RegisterKeyEditViewModel();

                RegisterKeyContent = new RegisterKeyEditView { DataContext = viewModel };
                viewModel.ViewModelInit(dialog,RegisterKeySubmitCallBack, Item, _client,_mapper);

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
        private void RegisterKeySubmitCallBack(bool success, string message, object data)
        {
            if (success)
            {
                dialog.Success("提示", message);
                IsEdit = false;
                RegisterKeyContent = null;
                CompanRegisterKeysLoad();
            }
            else if (!success && message == "关闭")
            {
                IsEdit = false;
                RegisterKeyContent = null;
            }
        }

        /// <summary>
        /// 状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        public async Task ItemSate(string type, AuthorRegisterKeyItemDto node)
        {
            try
            {
                if (type == "IsDelete")
                {
                    RegisterKeyStatePost(type, node.AuthorId, "");
                }
                else if (type == "IsEdit")
                {
                    RegisterKeysAddContent(node);
                }
                else
                {
                    if (node.IsLock == 1)
                    {
                        var data = await dialog.Prompt("请输入锁定原因", "确定");
                        if (data.Item1)
                        {
                            RegisterKeyStatePost(type, node.AuthorId, data.Item2);
                        }
                    }
                    else
                    {
                        RegisterKeyStatePost(type, node.AuthorId, "");
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 状态变更请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Id"></param>
        /// <param name="Reason"></param>
        private void RegisterKeyStatePost(string type, string Id, string Reason)
        {
            dialog.ShowLoading("数据处理中...", async e =>
            {
                AuthorRegisterKeyStateRequestDto request = new AuthorRegisterKeyStateRequestDto
                {
                    Type = type,
                    AuthorId = Id,
                    Reason = Reason
                };
                var data = await _client.AuthorRegisterKeyStateAsync(_mapper.Map<AuthorRegisterKeyStateRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    dialog.Success("提示", data.Message);
                    //刷新数据集
                    ReisterKeyList(Id);
                }
                else
                {
                    dialog.Error("提示", data.Message);
                }
            });
        }
        /// <summary>
        /// 数据集更新
        /// </summary>
        /// <param name="AuthorId"></param>
        private void ReisterKeyList(string AuthorId)
        {
            dialog.ShowLoading("数据重载中...", async e =>
            {
                try
                {
                    AuthorRegisterKeyListRequestDto request = new AuthorRegisterKeyListRequestDto
                    {
                        AuthorId=AuthorId
                    };
                    var response = await _client.AuthorRegisterKeyListAsync(_mapper.Map<AuthorRegisterKeyListRequest>(request), CommAction.GetHeader());
                    if (response.Code)
                    {
                        var list = _mapper.Map<List<AuthorRegisterKeyItemDto>>(response.Items);
                        ReginsterList = [.. list];
                        if (!string.IsNullOrEmpty(CompanySelected?.CompanyId ?? string.Empty))
                        {
                            CompanySelected.RegisterKeys = list;
                        }
                    }
                    else
                    {
                        dialog.Error("提示", response.Message);
                    }
                }
                catch (Exception ex)
                {
                    dialog.Error("异常", ex.Message.ToString());
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
    }
}
