using AutoMapper;
using Avalonia.Controls;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;

using bbnApp.deskTop.Models.Role;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Material.Icons;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Emit;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static bbnApp.Share.StaticModel;

namespace bbnApp.deskTop.OrganizationStructure.Role
{
    partial class RoleViewModel : BbnPageBase
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
        /// 图片加载状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 角色服务
        /// </summary>
        private RoleGrpc.RoleGrpcClient _client;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 角色List
        /// </summary>
        [ObservableProperty] private ObservableCollection<RoleItemDto> _roleListSource = new ObservableCollection<RoleItemDto>();
        /// <summary>
        /// 选中的角色代码
        /// </summary>
        [ObservableProperty] private RoleItemDto _roleSelected;
        /// <summary>
        /// 标准应用和权限
        /// </summary>
        [ObservableProperty] private ObservableCollection<RoleAppsModel> _roleDefaultApps = new ObservableCollection<RoleAppsModel>();
        /// <summary>
        /// 选中的角色代码的应用清单
        /// </summary>
        [ObservableProperty] private ObservableCollection<RoleAppsModel> _roleApps = new ObservableCollection<RoleAppsModel>();
        /// <summary>
        /// 选中角色的操作对象
        /// </summary>
        [ObservableProperty] private ObservableCollection<RolePermissionItemModel> _rolePermissions = new ObservableCollection<RolePermissionItemModel>();
        /// <summary>
        /// 选中角色的操作对象代码信息
        /// </summary>
        [ObservableProperty] private ObservableCollection<PermissionCodeItemModel> _roleCodes = new ObservableCollection<PermissionCodeItemModel>();
        /// <summary>
        /// 级别数据集
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _roleLeveTypeList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的职级
        /// </summary>
        [ObservableProperty] private ComboboxItem _roleLeveSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 选中角色变更时，自动加载角色详细信息
        /// </summary>
        /// <param name="value"></param>
        partial void OnRoleSelectedChanged(RoleItemDto item)
        {
            if (item != null)
            {
                if (!string.IsNullOrEmpty(item.RoleId))
                {
                    RoleInfoLoad(item);
                }
            }
        }
        /// <summary>
        /// 角色对象变更
        /// </summary>
        /// <param name="value"></param>
        partial void OnRoleAppsChanged(ObservableCollection<RoleAppsModel> value)
        {
            if (value != null && value.Count > 0)
            {
                var applist = value.Where(x => x.IsChecked).ToList();
                foreach (var app in applist)
                {
                    var items = app.Items;
                    foreach (var item in items)
                    {
                        RolePermissions.Add(item);
                    }
                }
            }
            else
            {
                RoleCodes.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public RoleViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("OrganizationStructure", "角色信息", MaterialIconKind.CardAccountDetails, "", 4)
        {
            _ = ClientInit(grpcClientFactory);
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _mapper = mapper;
            this.dialog = dialog;
        }
        /// <summary>
        /// 初始化Client
        /// </summary>
        /// <param name="grpcClientFactory"></param>
        /// <returns></returns>
        private async Task ClientInit(IGrpcClientFactory grpcClientFactory)
        {
            _client = await grpcClientFactory.CreateClient<RoleGrpc.RoleGrpcClient>();
        }
        /// <summary>
        /// 字典初始化
        /// </summary>
        public async Task RoleDicInit()
        {
            RoleLeveTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1016"));
           await RoleAppListLoad(() => {
                GetRoleListAsync();
            });
        }
        #region 角色列表
        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        private void RoleListLoad() {
            GetRoleListAsync();
        }
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        private void GetRoleListAsync()
        {
            dialog.ShowLoading("角色列表载入中...", async e =>
            {
                try
                {
                    var request = new RoleListRequestDto
                    {
                        RoleLeve = 0
                    };
                    var response = await _client.RoleListLoadAsync(_mapper.Map<RoleListRequest>(request), CommAction.GetHeader());
                    if (RoleListSource.Count > 0)
                    {
                        RoleListSource.Clear();
                    }
                    if (response.Code)
                    {
                        RoleListSource = new ObservableCollection<RoleItemDto>(_mapper.Map<List<RoleItemDto>>(response.Data));
                    }
                    else
                    {
                        dialog.Error("获取角色列表失败", response.Message);
                    }
                }
                catch (Exception ex)
                {
                    dialog.Error("获取角色列表异常", ex.Message);
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
        /// <summary>
        /// 获取选中的角色信息
        /// </summary>
        /// <param name="item"></param>
        public void RoleInfoLoad(RoleItemDto item)
        {
            if (item == null) return;
            dialog.ShowLoading("角色信息载入中...", async e =>
            {
                try
                {
                    var request = new RoleInfoRequestDto
                    {
                        RoleId = item.RoleId
                    };
                    var response = await _client.RoleInfoAsync(_mapper.Map<RoleInfoRequest>(request), CommAction.GetHeader());
                    if (response.Code)
                    {
                        RoleParse(_mapper.Map<List<RoleAppsDto>>(response.RoleApps));
                        SetDicValue();
                    }
                    else
                    {
                        dialog.Error("获取角色应用信息失败", response.Message);
                    }
                }
                catch (Exception ex)
                {
                    dialog.Error("获取角色应用信息异常", ex.Message);
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        private void RoleParse(List<RoleAppsDto> list,bool isdefault=false)
        {
            RoleApps.Clear();
            foreach (var data in list)
            {
                foreach (var permission in data.Items)
                {
                    var per = new RolePermissionItemModel
                    {
                        IdxNum = permission.IdxNum,
                        Yhid = permission.Yhid,
                        RoleId = permission.RoleId,
                        ObjCode = permission.ObjCode,
                        ObjName = permission.ObjName,
                        CompanyId = permission.CompanyId,
                        ObjDescription = permission.ObjDescription,
                        IsChecked = permission.IsChecked,
                        Codes = new ObservableCollection<PermissionCodeItemModel>(permission.Codes.Select(c => new PermissionCodeItemModel
                        {
                            IdxNum = c.IdxNum,
                            ObjCode = c.ObjCode,
                            ObjName = c.ObjName,
                            IsChecked = c.IsChecked,
                            RoleId = c.RoleId,
                            PermissionCode = c.PermissionCode,
                            PermissionName = c.PermissionName
                        }))
                    };
                }
                var app = new RoleAppsModel
                {
                    IdxNum = data.IdxNum,
                    Yhid = data.Yhid,
                    AppId = data.AppId,
                    RoleId = data.RoleId,
                    AppCode = data.AppCode,
                    CompanyId = data.CompanyId,
                    AppName = data.AppName,
                    IsChecked = data.IsChecked,
                    Items = new ObservableCollection<RolePermissionItemModel>(data.Items.Select(PermissionDtoToModel))
                };
                if (isdefault)
                {
                    RoleDefaultApps.Add(app);
                }
                else
                {
                    RoleApps.Add(app);
                }
            }

            permissionChecked();
        }
        /// <summary>
        /// 角色应用选择变更
        /// </summary>
        /// <param name="app"></param>
        public void AppChekced(RoleAppsModel app)
        {
            var existingItem = RoleApps.FirstOrDefault(x => x.RoleId == app.RoleId && x.AppId == app.AppId);
            if (existingItem != null)
            {
                existingItem.IsChecked = app.IsChecked;
            }
            else
            {
                // 如果不存在，则直接添加
                RoleApps.Add(app);
            }
            permissionChecked();
        }
        /// <summary>
        /// 设置权限勾选状态
        /// </summary>
        private void permissionChecked()
        {
            RolePermissions.Clear();
            var checkedApps = RoleApps.Where(x => x.IsChecked).ToList();
            foreach (var item in checkedApps)
            {
                var permissions = item.Items;
                foreach (var per in permissions)
                {
                    if (!RolePermissions.Any(x => x.ObjCode == per.ObjCode))
                    {
                        RolePermissions.Add(per);
                    }
                    else
                    {
                        RolePermissions.Remove(per);
                    }
                }
            }
        }
        /// <summary>
        /// 全选状态变更
        /// </summary>
        /// <param name="item"></param>
        public void PermissionChange(RolePermissionItemModel item)
        {
            var existingItem = RolePermissions.FirstOrDefault(x => x.ObjCode == item.ObjCode);
            if (existingItem != null)
            {
                foreach (var code in existingItem.Codes)
                {
                    if (code.IsChecked != existingItem.IsChecked)
                    {
                        code.IsChecked = existingItem.IsChecked;
                    }
                }
            }
        }
        /// <summary>
        /// 权限勾选
        /// </summary>
        /// <param name="item"></param>
        public void CodeChekced(PermissionCodeItemModel item)
        {
            //主要是控制全选状态
            var existingItem = RolePermissions.FirstOrDefault(x => x.ObjCode == item.ObjCode);
            if (existingItem != null)
            {
                if (!item.IsChecked && existingItem.IsChecked)
                {
                    //撤销全选
                    existingItem.IsChecked = false;
                }
                else if (item.IsChecked && !existingItem.IsChecked)
                {
                    //判断子项是否全部都选中了
                    if (!existingItem.Codes.Any(x => !x.IsChecked))
                    {
                        existingItem.IsChecked = true;
                    }
                }
            }
        }
        /// <summary>
        /// 设置字典值
        /// </summary>
        private void SetDicValue()
        {
            RoleLeveSelected = CommAction.SetSelectedItem(RoleLeveTypeList, RoleSelected?.ReMarks);
        }
        #endregion
        #region 角色维护
        [RelayCommand]
        private async Task RoleAdd()
        {
            RoleSelected = new RoleItemDto();
            await Task.Delay(100);
            RoleSelected =new RoleItemDto
            {
                RoleName = "新角色",
                ReMarks = "",
                RoleLeve = Convert.ToByte(UserContext.CurrentUser?.PositionLeve ??9),
                RoleDescription= "新角色的描述"
            };
            SetDicValue();
            //清空选中
            RoleApps = RoleDefaultApps;
        }
        /// <summary>
        /// 获取标准应用和权限数据
        /// </summary>
        private async Task RoleAppListLoad(Action ac)
        {
            try
            {
                var request = new RoleAppListRequestDto
                {
                    RoleId = string.Empty,
                    CompanyId=UserContext.CurrentUser?.CompanyId ?? string.Empty,
                };
                var response=await _client.RoleAppListLoadAsync(_mapper.Map<RoleAppListRequest>(request), CommAction.GetHeader());
                if (response.Code)
                {

                    RoleParse(_mapper.Map<List<RoleAppsDto>>(response.Data),true);
                    ac();
                }
                else
                {
                    dialog.Error("获取标准应用和权限数据失败", response.Message);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("获取标准应用和权限数据异常", ex.Message);
            }
        }
        /// <summary>
        /// 重置勾选状态
        /// </summary>
        /// <param name="roleApps"></param>
        public void ResetAllRoleApps(ObservableCollection<RoleAppsModel> roleApps)
        {
            foreach (var roleApp in roleApps)
            {
                roleApp.ResetAllChecked();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleApps"></param>
        public void ResetAllPermission(ObservableCollection<RoleAppsModel> roleApps)
        {
            foreach (var roleApp in roleApps)
            {
                foreach (var item in roleApp.Items)
                {
                    item.ResetAllChecked();
                }
            }
        }
        /// <summary>
        /// 清楚权限选中
        /// </summary>
        [RelayCommand]
        private void CheckedClear()
        {
            ResetAllPermission(RoleApps);
        }
        /// <summary>
        /// 应用重载
        /// </summary>
        [RelayCommand]
        private void RoleAppReload()
        {
            if (RoleSelected != null)
            {
                RoleInfoLoad(RoleSelected);
            }
        }
        /// <summary>
        /// 角色信息保存
        /// </summary>
        [RelayCommand]
        private void RoleSave()
        {
            dialog.ShowLoading("角色信息保存中...", async e =>
            {
                try
                {
                    if (RoleSelected == null) return;
                    RoleSelected.ReMarks = RoleLeveSelected?.Id ?? string.Empty;

                    var request = new RoleSaveRequestDto
                    {
                        Role=RoleSelected, 
                        RoleApps= AppsParse(RoleApps)
                    };
                    var response = await _client.RolePostAsync(_mapper.Map<RoleSaveRequest>(request), CommAction.GetHeader());
                    if (response.Code)
                    {
                        dialog.Success("角色信息保存成功");
                        if (string.IsNullOrEmpty(RoleSelected.RoleId))
                        {
                            GetRoleListAsync();
                        }
                        RoleSelected = _mapper.Map<RoleItemDto>(response.Role);
                    }
                    else
                    {
                        dialog.Error("角色信息保存失败", response.Message);
                    }
                }
                catch (Exception ex)
                {
                    dialog.Error("角色信息保存异常", ex.Message);
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
        /// <summary>
        /// 角色状态变更
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="item"></param>
        public async Task RoleState(string type, RoleItemDto node)
        {
            try
            {
                if (type == "IsDelete")
                {
                    RoleStatePost(type, node.RoleId, "");
                }
                else
                {
                    if (node.IsLock==Convert.ToByte(0))
                    {
                        var data = await dialog.Prompt("请输入锁定原因", "确定");
                        if (data.Item1)
                        {
                            RoleStatePost(type, node.RoleId, data.Item2);
                        }
                    }
                    else
                    {
                        RoleStatePost(type, node.RoleId, "");
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 角色状态变更请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Id"></param>
        /// <param name="Reason"></param>
        private void RoleStatePost(string type, string Id, string Reason)
        {
            dialog.ShowLoading("数据处理中...", async e => {
                try
                {
                    RoleStateRequestDto request = new RoleStateRequestDto
                    {
                        Type = type,
                        RoleId = Id,
                        Reason = Reason
                    };
                    var data = await _client.RoleStateAsync(_mapper.Map<RoleStateRequest>(request), CommAction.GetHeader());
                    if (data.Code)
                    {
                        dialog.Success("提示", data.Message);
                        //刷新
                        GetRoleListAsync();
                        if (type == "IsDelete")
                        {
                            RoleSelected = new RoleItemDto();
                            RoleApps = new ObservableCollection<RoleAppsModel>();
                            RolePermissions = new ObservableCollection<RolePermissionItemModel>();
                            RoleCodes = new ObservableCollection<PermissionCodeItemModel>();
                        }
                        else
                        {
                            RoleInfoLoad(_mapper.Map<RoleItemDto>(data.Data));
                        }
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }
                }
                catch(Exception ex)
                {
                    dialog.Error("提示", ex.Message.ToString());
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
        #endregion
        #region 角色对象转换Dto→Model
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private RolePermissionItemModel PermissionDtoToModel(RolePermissionItemDto dto)
        {
             
            return new RolePermissionItemModel
            {
                IdxNum = dto.IdxNum,
                Yhid = dto.Yhid,
                RoleId = dto.RoleId,
                ObjCode = dto.ObjCode,
                ObjName = dto.ObjName,
                CompanyId = dto.CompanyId,
                ObjDescription = dto.ObjDescription,
                IsChecked = dto.IsChecked,
                Codes = PermissonCodeDtoToModel(dto.Codes),
            };
        }
        private ObservableCollection<PermissionCodeItemModel> PermissonCodeDtoToModel(List<PermissionCodeItemDto> list)
        {
            ObservableCollection<PermissionCodeItemModel> data = new ObservableCollection<PermissionCodeItemModel>();
            foreach(var item in list)
            {
                data.Add(new PermissionCodeItemModel { 
                    IdxNum=item.IdxNum,
                    ObjCode=item.ObjCode,
                    ObjName=item.ObjName,
                    IsChecked=item.IsChecked,
                    RoleId=item.RoleId,
                    PermissionCode=item.PermissionCode,
                    PermissionName=item.PermissionName
                });
            }
            return data;
        }
        #endregion
        #region 角色对象Model→Dto
        /// <summary>
        /// app对象转换
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        private List<RoleAppsDto> AppsParse(ObservableCollection<RoleAppsModel> apps)
        {
            List<RoleAppsDto> list = new List<RoleAppsDto>();
            foreach (var app in apps)
            {
                if (app.IsChecked)
                {
                    var dto = new RoleAppsDto
                    {
                        IdxNum = app.IdxNum,
                        Yhid = app.Yhid,
                        AppId = app.AppId,
                        RoleId = app.RoleId,
                        AppCode = app.AppCode,
                        CompanyId = app.CompanyId,
                        AppName = app.AppName,
                        IsChecked = app.IsChecked,
                        Items = [.. app.Items.Where(x=>x.IsChecked).Select(permission=>new RolePermissionItemDto {
                        IdxNum = permission.IdxNum,
                        Yhid = permission.Yhid,
                        RoleId = permission.RoleId,
                        ObjCode = permission.ObjCode,
                        ObjName = permission.ObjName,
                        CompanyId = permission.CompanyId,
                        ObjDescription = permission.ObjDescription,
                        IsChecked = permission.IsChecked,
                        Codes =[.. permission.Codes.Where(x=>x.IsChecked).Select(code => new PermissionCodeItemDto
                        {
                            IdxNum = code.IdxNum,
                            ObjCode = code.ObjCode,
                            ObjName = code.ObjName,
                            IsChecked = code.IsChecked,
                            RoleId = code.RoleId,
                            PermissionCode = code.PermissionCode,
                            PermissionName = code.PermissionName
                        })]
                    })]
                    };
                    list.Add(dto);
                }
            }
            return list;          
        }
        #endregion
    }
}
