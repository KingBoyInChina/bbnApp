using AutoMapper;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Styling;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Models.User;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.MQTT.Client;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.UserCenter.UserInformation
{
    partial class UserInformationViewModel : BbnPageBase
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
        /// 密钥
        /// </summary>
        private UserInformationGrpc.UserInformationGrpcClient _client;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 用户类型
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _userType = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 表单选中的
        /// </summary>
        [ObservableProperty] private ComboboxItem _userSelectedType = new ComboboxItem("","","");
        /// <summary>
        /// 用户级别
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _userLeve = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 种养分类 1218
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _aabType = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 种养类型 1203+1217
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _categorys = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 计量单位 1209
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _units = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 分布情况 1219
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _distributionDatas = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 规模情况 1220
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _scales = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的规模
        /// </summary>
        [ObservableProperty] private ComboboxItem _userSelectedScale = new ComboboxItem("", "", "");
        
        /// <summary>
        /// 选中的用户级别
        /// </summary>
        [ObservableProperty] private ComboboxItem _userSelectedLeve = new ComboboxItem("", "", "");
        /// <summary>
        /// 用户tree
        /// </summary>
        [ObservableProperty] private ObservableCollection<UserTreeItemDto> _userTreeSource = new ObservableCollection<UserTreeItemDto>();
        /// <summary>
        /// 查询条件-行政区划代码
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _areaFilterSelected;
        /// <summary>
        /// 表单行政地区选中
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _areaSelected;
        /// <summary>
        /// 表单默认行政区划默认选中
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _initValue;
        /// <summary>
        /// 查询-默认选中项
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _initFilterValue;
        /// <summary>
        /// 查询-联系电话
        /// </summary>
        [ObservableProperty] private string _filterPhoneNumber=string.Empty;
        /// <summary>
        /// 查询-用户名称
        /// </summary>
        [ObservableProperty] private string _filterUserName = string.Empty;
        /// <summary>
        /// 选中用户节点
        /// </summary>
        [ObservableProperty] private UserTreeItemDto _selectedNode = new UserTreeItemDto();
        /// <summary>
        /// 节点选中
        /// </summary>
        /// <param name="value"></param>
        partial void OnSelectedNodeChanged(UserTreeItemDto node)
        {
            try
            {
                if (node != null)
                {
                    if ((bool)node.IsLeaf)
                    {
                        NodeInfoLoad(node);
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 用户信息
        /// </summary>
        [ObservableProperty] private UserInformationDto _userInformation = new UserInformationDto();
        /// <summary>
        /// 用户联系人信息
        /// </summary>
        [ObservableProperty] private ObservableCollection<UserContactDto> _contacts= new ObservableCollection<UserContactDto>();
        /// <summary>
        /// 种养信息
        /// </summary>
        [ObservableProperty] private ObservableCollection<UserAabItem> _aabs = new ObservableCollection<UserAabItem>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public UserInformationViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog, MqttClientService _mqttClientService) : base("UserCenter", "用户管理", MaterialIconKind.Users, "", 1)
        {
            _ = ClientInit(grpcClientFactory);
            this.dialogManager = DialogManager;

            this.nav = nav;
            this.dialog = dialog;
            _mapper = mapper;
        }
        /// <summary>
        /// 初始化Client
        /// </summary>
        /// <param name="grpcClientFactory"></param>
        /// <returns></returns>
        private async Task ClientInit(IGrpcClientFactory grpcClientFactory)
        {
            _client = await grpcClientFactory.CreateClient<UserInformationGrpc.UserInformationGrpcClient>();
        }
        /// <summary>
        /// 字典初始化
        /// </summary>
        public void DataInit()
        {
            UserType = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1215"));
            UserLeve = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1216"));
            Units = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1209"));
            AabType = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1218"));
            DistributionDatas = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1219"));
            Scales = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1220"));
            var items1 = CommAction.GetDicItems("1203");
            var item2 = CommAction.GetDicItems("1217");
            var items =items1.Concat(item2);
            Categorys = new ObservableCollection<ComboboxItem>(items);
            InitValue = new AreaTreeNodeDto
            {
                AreaId = UserContext.CurrentUser.AreaCode,
                AreaFullName = UserContext.CurrentUser.AreaName,
                AreaName = UserContext.CurrentUser.AreaName
            };
            AreaSelected = new AreaTreeNodeDto
            {
                AreaId = UserContext.CurrentUser.AreaCode,
                AreaFullName = UserContext.CurrentUser.AreaName,
                AreaName = UserContext.CurrentUser.AreaName
            };
            AreaFilterSelected = new AreaTreeNodeDto
            {
                AreaId = UserContext.CurrentUser.AreaCode,
                AreaFullName = UserContext.CurrentUser.AreaName,
                AreaName = UserContext.CurrentUser.AreaName
            };
            InitFilterValue = new AreaTreeNodeDto
            {
                AreaId = UserContext.CurrentUser.AreaCode,
                AreaFullName = UserContext.CurrentUser.AreaName,
                AreaName = UserContext.CurrentUser.AreaName
            };
            UserLoadCommand.Execute(null);
        }
        /// <summary>
        /// 读取节点信息
        /// </summary>
        /// <param name="node"></param>
        private void NodeInfoLoad(UserTreeItemDto node)
        {
            dialog.ShowLoading("数据加载中...", async (e) =>
            {
                try
                {
                    var request = new UserInformationLoadRequestDto
                    {
                        UserId = node.Id
                    };
                    var data = await _client.UserInformationLoadAsync(_mapper.Map<UserInformationLoadRequest>(request),CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        UserInformation = _mapper.Map<UserInformationDto>(data.User);
                        var contactlist = _mapper.Map<List<UserContactDto>>(data.Contacts);
                        Contacts = new ObservableCollection<UserContactDto>(contactlist);
                        var list = _mapper.Map<List<UserAabInformationDto>>(data.Aabs);
                        Aabs =[.. userAabItems(list)];
                        DicInit();
                    }
                    else
                    {
                        dialog.Error("提示", $"用户信息读取失败：{data.Message}");
                    }
                }
                catch (Exception ex)
                {
                    dialog.LoadingClose(e);
                    dialog.Error("提示", $"用户信息读取异常：{ex.Message.ToString()}");
                }
            });
        }
        /// <summary>
        /// 初始化字典值
        /// </summary>
        private void DicInit()
        {
            UserSelectedType = CommAction.SetSelectedItem(UserType, UserInformation.UserType); 
            UserSelectedLeve = CommAction.SetSelectedItem(UserLeve, UserInformation.UserLeve);
            UserSelectedScale = CommAction.SetSelectedItem(Scales, UserInformation.Scale);
        }
        /// <summary>
        /// 用户查询
        /// </summary>
        [RelayCommand]
        private void UserLoad()
        {
            dialog.ShowLoading("数据查询中...",async e =>
            {
                try
                {
                    var request = new UserInformationTreeRequestDto { 
                        AreaId= AreaFilterSelected.AreaId,
                        PhoneNumber=FilterPhoneNumber,
                        UserName=FilterUserName
                    };
                    var data = await _client.UserInformationTreeAsync(_mapper.Map<UserInformationTreeRequest>(request),CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        UserTreeSource =[.._mapper.Map<List<UserTreeItemDto>>(data.Items)];
                    }
                    else
                    {
                        dialog.Error("数据查询失败",data.Message);
                    }
                }
                catch(Exception ex)
                {
                    dialog.LoadingClose(e);
                    dialog.Error("数据查询异常",ex.Message.ToString());
                }
            });
        }
        /// <summary>
        /// 用户新增
        /// </summary>
        [RelayCommand]
        private void UserAddAction()
        {
            UserInformation = new UserInformationDto();
            Contacts.Clear();
            Aabs.Clear();
        }
        /// <summary>
        /// 添加联系人
        /// </summary>
        [RelayCommand]
        private void ContactAddAction() {
            var contanct = new UserContactDto {
                IdxNum = Contacts.Count + 1,
                IsFirst = Contacts.Count == 0 ?true:false
            };
            Contacts.Add(contanct);
        }
        /// <summary>
        /// 种养信息添加
        /// </summary>
        [RelayCommand]
        private void AabAddAction()
        {
            var aabitem = new UserAabItem
            {
                IdxNum = Aabs.Count + 1,
                IsLock = (byte)0,
                AABTypes = AabType,
                AABTypeSelected = CommAction.SetSelectedItem(AabType, ""),
                CategoriSelected = CommAction.SetSelectedItem(Categorys, ""),
                Categoris = Categorys,
                DistributionSelected = CommAction.SetSelectedItem(DistributionDatas, ""),
                Distributions = DistributionDatas,
                AreaNumber=10,
                AreaNumberUnitSelected = CommAction.SetSelectedItem(Units, "亩"),
                AreaNumberUnits = Units
            };
            Aabs.Add(aabitem);
        }
        /// <summary>
        /// 用户提交
        /// </summary>
        [RelayCommand]
        private void UserSaveAction()
        {
            #region 提交对象赋值
            UserInformation.AreaId = AreaSelected?.AreaId ?? string.Empty;
            UserInformation.AreaName = AreaSelected?.AreaName ?? string.Empty;
            UserInformation.UserType = UserSelectedType?.Id ?? string.Empty;
            UserInformation.UserLeve = UserSelectedLeve?.Id ?? string.Empty;
            UserInformation.Scale = UserSelectedScale?.Id ?? string.Empty;
            #endregion
            #region 逻辑校验
            StringBuilder _error = new StringBuilder();
            if (string.IsNullOrEmpty(UserInformation.UserName))
            {
                _error.AppendLine("用户名称不能为空");
            }
            if (string.IsNullOrEmpty(UserInformation.UserType))
            {
                _error.AppendLine("用户分类不能为空");
            }
            if (string.IsNullOrEmpty(UserInformation.UserLeve))
            {
                _error.AppendLine("用户级别不能为空");
            }
            if (string.IsNullOrEmpty(UserInformation.Scale))
            {
                _error.AppendLine("规模不能为空");
            }
            if (string.IsNullOrEmpty(UserInformation.AreaId))
            {
                _error.AppendLine("所在地区不能为空");
            }
            if (string.IsNullOrEmpty(UserInformation.Address))
            {
                _error.AppendLine("通信地址不能为空");
            }
            if (Contacts.Count == 0)
            {
                _error.AppendLine("联系人不能为空");
            }
            if (Aabs.Count == 0)
            {
                _error.AppendLine("种养信息不能为空");
            }
            #endregion
            if (!string.IsNullOrEmpty(_error.ToString()))
            {
                dialog.Error("逻辑错误",_error.ToString());
                return;
            }
            dialog.ShowLoading("数据提交中...",async e =>
            {
                try
                {
                    var aabs = userAabDtos(Aabs);
                    var contacts = Contacts.ToList();
                    var user = UserInformation;
                    var request = new UserInformationSaveRequestDto
                    {
                        User = user,
                        Contacts = contacts,
                        Aabs = aabs
                    };
                    var data =await _client.UserInformationSaveAsync(_mapper.Map<UserInformationSaveRequest>(request),CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        dialog.Success("提示",data.Message);
                        UserInformation = _mapper.Map<UserInformationDto>(data.User);
                        Contacts = [.. _mapper.Map<List<UserContactDto>>(data.Contacts)];
                        var list = _mapper.Map<List<UserAabInformationDto>>(data.Aabs);
                        Aabs = [.. userAabItems(list)];
                    }
                    else
                    {
                        dialog.Error("提交失败",data.Message);
                    }
                }
                catch(Exception ex)
                {
                    dialog.LoadingClose(e);
                    dialog.Error("提交异常",$"用户信息提交异常：{ex.Message.ToString()}");
                }
            });
        }
        /// <summary>
        /// 用户状态变更
        /// </summary>
        public async Task UserState(string tag, string userid, string contactid, string aabid,bool IsLock)
        {
            if ((!IsLock && tag=="IsLock")||(IsLock&&tag.Contains("Lock")&&tag!="IsLock"))
            {
                var data = await dialog.Prompt("请输入锁定原因", "确定");
                if (data.Item1)
                {
                    StateChange(tag, userid, contactid, aabid,data.Item2);
                }
            }
            else
            {
                StateChange(tag,userid,contactid,aabid,"");
            }
        }
        /// <summary>
        /// 状态提交
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="userid"></param>
        /// <param name="contactid"></param>
        /// <param name="aabid"></param>
        /// <param name="reason"></param>
        private void StateChange(string tag,string userid,string contactid,string aabid,string reason)
        {
            dialog.ShowLoading("数据提交中...",async e =>
            {
                try
                {
                    var request = new UserInformationStateRequestDto
                    {
                        AabId = aabid,
                        Reason = reason,
                        UserId = userid,
                        ContactId = contactid,
                        Type = tag
                    };
                    var data = await _client.UserInformationStateAsync(_mapper.Map<UserInformationStateRequest>(request),CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        dialog.Success("提示",data.Message);
                        if (tag == "IsDelete")
                        {
                            UserInformation = new UserInformationDto();
                            Contacts.Clear();
                            Aabs.Clear();
                        }
                        else
                        {
                            if (tag == "IsLock")
                            {
                                SelectedNode.IsLock = !SelectedNode.IsLock;
                            }
                            UserInformation = _mapper.Map<UserInformationDto>(data.User);
                            Contacts = [.. _mapper.Map<List<UserContactDto>>(data.Contacts)];
                            var list = _mapper.Map<List<UserAabInformationDto>>(data.Aabs);
                            Aabs = [.. userAabItems(list)];
                        }
                    }
                    else
                    {
                        dialog.Error("提交失败",data.Message);
                    }
                }
                catch(Exception ex)
                {
                    dialog.LoadingClose(e);
                    dialog.Error("提交异常",$"数据提交异常：{ex.Message.ToString()}");
                }
            });
        }
        #region 对象处理
        /// <summary>
        /// 种养信息dto→item
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private UserAabItem UserAabDtoToItem(UserAabInformationDto dto)
        {
            return new UserAabItem
            {
                IdxNum = dto.IdxNum,
                AabId = dto.AabId,
                UserId = dto.UserId,
                AABType = dto.AABType,
                Categori = dto.Categori,
                ObjName = dto.ObjName,
                ObjCode = dto.ObjCode,
                AreaNumber = dto.AreaNumber,
                AreaNumberUnit = dto.AreaNumberUnit,
                Distribution = dto.Distribution,
                Point = dto.Point,
                IsLock = dto.IsLock,
                LockTime = dto.LockTime,
                LockReason = dto.LockReason,
                ReMarks = dto.ReMarks,
                AABTypes=AabType,
                AABTypeSelected = CommAction.SetSelectedItem(AabType, dto.AABType),
                CategoriSelected = CommAction.SetSelectedItem(Categorys, dto.Categori),
                Categoris=Categorys,
                DistributionSelected = CommAction.SetSelectedItem(DistributionDatas, dto.Distribution),
                Distributions= DistributionDatas,
                AreaNumberUnitSelected = CommAction.SetSelectedItem(Units, dto.AreaNumberUnit),
                AreaNumberUnits= Units
            };
        }

        private UserAabInformationDto UserAabItemToDto(UserAabItem dto)
        {
            return new UserAabInformationDto {
                IdxNum = dto.IdxNum,
                AabId = dto.AabId,
                UserId = dto.UserId,
                AABType = dto.AABTypeSelected?.Id??string.Empty,
                Categori = dto.CategoriSelected?.Id ?? string.Empty,
                ObjName = dto.ObjName,
                ObjCode = dto.ObjCode,
                AreaNumber = dto.AreaNumber,
                AreaNumberUnit = dto.AreaNumberUnitSelected?.Id ?? string.Empty,
                Distribution = dto.DistributionSelected?.Id ?? string.Empty,
                Point = dto.Point,
                IsLock = dto.IsLock,
                LockTime = dto.LockTime,
                LockReason = dto.LockReason,
                ReMarks = dto.ReMarks
            };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private List<UserAabItem> userAabItems(List<UserAabInformationDto> dto)
        {
            return dto.Select(x=>UserAabDtoToItem(x)).ToList();
        }

        private List<UserAabInformationDto> userAabDtos(ObservableCollection<UserAabItem> items)
        {
            return items.Select(x => UserAabItemToDto(x)).ToList();
        }
        #endregion
    }

}
