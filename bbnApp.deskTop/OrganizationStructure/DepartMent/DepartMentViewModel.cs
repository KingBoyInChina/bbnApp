using AutoMapper;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;

using bbnApp.deskTop.Services;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.Protos;
using bbnApp.Share;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Net.ClientFactory;
using Material.Icons;
using Org.BouncyCastle.Utilities;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.OrganizationStructure.DepartMent
{
	partial class DepartMentViewModel : BbnPageBase
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
        /// 部门代码服务
        /// </summary>
        private DepartMentGrpc.DepartMentGrpcClient _client;
        /// <summary>
        /// 公司代码服务
        /// </summary>
        private CompanyGrpcService.CompanyGrpcServiceClient _companyClient;
        /// <summary>
        /// 文件上传服务
        /// </summary>
        private UploadFileGrpc.UploadFileGrpcClient _uploadClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 部门树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<DepartMentTreeItemDto> _departMentTreeSource = new ObservableCollection<DepartMentTreeItemDto>();
        /// <summary>
        /// 父级公司代码
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _companyListSource = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的公司代码
        /// </summary>
        [ObservableProperty] private ComboboxItem _companySelected = new ComboboxItem("","","");


        /// <summary>
        /// 自动生成的局部方法，会在属性值变更时被调用
        /// </summary>
        /// <param name="value"></param>
        partial void OnCompanySelectedChanged(ComboboxItem item)
        {
            if (item != null)
            {
                DepartMentTreeLoad();
                _=GeDepartMentItems();
            }
        }
        /// <summary>
        /// 选中的树节点
        /// </summary>
        private DepartMentTreeItemDto _selectedTreeNode = new DepartMentTreeItemDto();
        /// <summary>
        /// 选中的部门
        /// </summary>
        [ObservableProperty] private DepartMentInfoDto _departMentSelected = new DepartMentInfoDto();
        /// <summary>
        /// 部门清单
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _departMentList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 表单，部门所属上级部门
        /// </summary>
        [ObservableProperty] private ComboboxItem _pDepartMentSelected = new ComboboxItem("","","");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public DepartMentViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("OrganizationStructure", "部门信息", MaterialIconKind.OfficeBuilding, "", 2)
        {
            _ = ClientInit(grpcClientFactory);
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _mapper = mapper;
            this.dialog = dialog;
        }

        private async Task ClientInit(IGrpcClientFactory grpcClientFactory)
        {
            _client =await grpcClientFactory.CreateClient<DepartMentGrpc.DepartMentGrpcClient>();
            _companyClient = await grpcClientFactory.CreateClient<CompanyGrpcService.CompanyGrpcServiceClient>();
            _uploadClient = await grpcClientFactory.CreateClient<UploadFileGrpc.UploadFileGrpcClient>();
        }
        /// <summary>
        /// 初始化字典
        /// </summary>
        public async Task DepartMentDicInit(UserControl uc)
        {
            NowControl = uc;//当前控件
            await GetCompanyItems();
            await Task.Delay(200);

            CompanySelected = CommAction.SetSelectedItem(CompanyListSource, UserContext.CurrentUser.CompanyId);
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
        /// 公司部门清单
        /// </summary>
        /// <returns></returns>
        private async Task GeDepartMentItems()
        {
            try
            {
                DepartMentSearchRequestDto request = new DepartMentSearchRequestDto
                {
                    CompanyId = CompanySelected.Id,
                    DepartMentName=string.Empty
                };
                var data = await _client.GetDepartMentListAsync(_mapper.Map<DepartMentSearchRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    var list = _mapper.Map<List<DepartMentInfoDto>>(data.Items);
                    list.Add(new DepartMentInfoDto { DepartMentId="-1",DepartMentName="无",CompanyId= CompanySelected.Id});
                    List<ComboboxItem> items = list.Select(x => new ComboboxItem (x.DepartMentId, x.DepartMentName, x.CompanyId )).ToList();
                    DepartMentList = new ObservableCollection<ComboboxItem>(items);
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
        /// 公司树加载
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
                    var data = await _client.GetDepartMentTreeAsync(_mapper.Map<DepartMentTreeRequest>(request), CommAction.GetHeader());
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
        /// 树选中
        /// </summary>
        /// <param name="node"></param>
        public void TreeSelecte(DepartMentTreeItemDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if ((bool)node.IsLeaf)
                {
                    if (DepartMentSelected == null || DepartMentSelected?.DepartMentId != node.Id)
                    {
                        ImageClear();
                        NodeInfoLoad(node);
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 公司信息
        /// </summary>
        private void NodeInfoLoad(DepartMentTreeItemDto node)
        {
            dialog.ShowLoading("数据读取中...", async e => {
                DepartMentInfoRequestDto request = new DepartMentInfoRequestDto { DepartMentId = node.Id,CompanyId=node.Tag };
                var header = CommAction.GetHeader();
                var data = await _client.GetDepartMentInfoAsync(_mapper.Map<DepartMentInfoRequest>(request), header);
                dialog.LoadingClose(e);
                if (data.Code)
                {
                    DepartMentSelected = _mapper.Map<DepartMentInfoDto>(data.Item);
                    SetItemSelected();
                    if (!string.IsNullOrEmpty(DepartMentSelected.DepartMentId))
                    {
                        await FileRead(DepartMentSelected.DepartMentId);
                    }
                }
                else
                {
                    dialog.Error("提示", data.Message);
                }
            });

        }
        /// <summary>
        /// 设置选中项目
        /// </summary>
        private void SetItemSelected()
        {
            PDepartMentSelected = CommAction.SetSelectedItem(DepartMentList, DepartMentSelected.PDepartMentId);
        }
        /// <summary>
        /// 显示图片
        /// </summary>
        [ObservableProperty] private Bitmap _departMentImage;
        /// <summary>
        /// 是否有图片
        /// </summary>
        [ObservableProperty] private bool _imageShow = false;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private FileItemsDto _departMentImageInfo = new FileItemsDto();
        /// <summary>
        /// 待上传的文件请求对象
        /// </summary>
        private UploadFileRequestDto request = null;

        /// <summary>
        /// 图片信息清空
        /// </summary>
        private void ImageClear()
        {
            ImageShow = false;
            if (DepartMentImage != null)
            {
                DepartMentImage.Dispose();
                DepartMentImage = null;
            }
            if (request != null)
            {
                request = null;
            }
            DepartMentImageInfo = new FileItemsDto();
        }
        /// <summary>
        /// 文件选择
        /// </summary>
        [RelayCommand]
        private async Task FileSeleced()
        {
            try
            {
                if (DepartMentSelected != null && !string.IsNullOrEmpty(DepartMentSelected.DepartMentId))
                {
                    var data = await dialog.FileSelected(NowControl, "image");
                    if (data.Item1)
                    {
                        DepartMentImage = data.Item4;
                        byte[] imagebytes = data.Item3;
                        var header = CommAction.GetHeader();

                        FileItemsDto items = new FileItemsDto
                        {
                            FileBytes = imagebytes,
                            FileId = DepartMentImageInfo.FileId,
                            FileExt = data.Item5.Extension,
                            FileName = data.Item5.Name
                        };

                        UploadFileItemDto fileData = new UploadFileItemDto
                        {
                            ReMarks = (imagebytes.Length / 1024).ToString("0") + "Kb",
                            LinkKey = DepartMentSelected.DepartMentId,
                            LinkTable = "departments",
                            Files = new List<FileItemsDto> { items }
                        };

                        request = new UploadFileRequestDto
                        {
                            Item = fileData
                        };
                        bool post = await dialog.Confirm("提示", "是否上传当前文件？", "上传", "取消");
                        if (post)
                        {
                            FileUploadCommand.Execute(null);
                        }
                    }
                }
                else
                {
                    dialog.Error("提示", "请先选择物资信息");
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task FileUpload()
        {
            try
            {
                if (request != null)
                {
                    var response = await _uploadClient.UploadFilePostAsync(_mapper.Map<UploadFileRequest>(request), CommAction.GetHeader());
                    if (response.Code)
                    {
                        dialog.Success("提示", response.Message);
                        request = null;
                    }
                    else
                    {
                        dialog.Error("提示", response.Message);
                    }
                }
                else
                {
                    dialog.Error("提示", "请先选择需要上传的文件");
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkKey"></param>
        /// <param name="linkTable"></param>
        /// <returns></returns>
        private async Task FileRead(string linkKey, string linkTable = "departments")
        {
            try
            {
                IsBusy = true;
                var header = CommAction.GetHeader();
                UploadFileReadRequestDto request = new UploadFileReadRequestDto
                {
                    LinkKey = linkKey,
                    LinkTable = linkTable
                };

                var response = await _uploadClient.UploadFileReadAsync(_mapper.Map<UploadFileReadRequest>(request), header);
                if (response.Code)
                {
                    UploadFileItemDto item = _mapper.Map<UploadFileItemDto>(response.Item);
                    if (item != null)
                    {
                        DepartMentImageInfo = item.Files[0];
                        DepartMentImage = CommAction.ByteArrayToBitmap(DepartMentImageInfo.FileBytes);
                    }
                }
                ImageShow = true;
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        /// <summary>
        /// 新建部门
        /// </summary>
        [RelayCommand]
        private async Task AddDepartMent()
        {
            DepartMentSelected = new DepartMentInfoDto();
            await Task.Delay(200);

            DepartMentSelected.IdxNum = 0;
            ImageClear();
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private void DepartMentSave()
        {
            try
            {
                StringBuilder _error = new StringBuilder();
                DepartMentSelected.PDepartMentId = PDepartMentSelected?.Id;
                if (string.IsNullOrEmpty(DepartMentSelected.DepartMentName))
                {
                    _error.AppendLine($"部门名称不能为空");
                }
                if (!string.IsNullOrEmpty(_error.ToString()))
                {
                    dialog.Error("提示", _error.ToString());
                    return;
                }

                dialog.ShowLoading("数据提交中...", async e => {
                    try
                    {
                        bool b = false;
                        if (string.IsNullOrEmpty(DepartMentSelected.DepartMentId))
                        {
                            b = true ;
                        }
                        DepartMentSaveRequestDto request = new DepartMentSaveRequestDto
                        {
                            Item = DepartMentSelected
                        };
                        var header = CommAction.GetHeader();
                        var data = await _client.SaveDepartMentAsync(_mapper.Map<DepartMentSaveRequest>(request), header);

                        if (data.Code)
                        {
                            dialog.Success("提示", data.Message);

                            DepartMentSelected = _mapper.Map<DepartMentInfoDto>(data.Item);
                            ImageShow = true;
                            if (b)
                            {
                                DepartMentTreeLoad();
                            }
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
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 公司状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        public async Task DepartMentState(string type, DepartMentTreeItemDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if (type == "IsDelete")
                {
                    DepartMentStatePost(type, node.Id, "");
                }
                else
                {
                    if (!(bool)node.IsLock)
                    {
                        var data = await dialog.Prompt("请输入锁定原因", "确定");
                        if (data.Item1)
                        {
                            DepartMentStatePost(type, node.Id, data.Item2);
                        }
                    }
                    else
                    {
                        DepartMentStatePost(type, node.Id, "");
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 公司状态变更请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Id"></param>
        /// <param name="Reason"></param>
        private void DepartMentStatePost(string type, string Id, string Reason)
        {
            dialog.ShowLoading("数据处理中...", async e => {
                DepartMentStateRequestDto request = new DepartMentStateRequestDto
                {
                    Type = type,
                    DepartMentId = Id,
                    Reason = Reason
                };
                var data = await _client.StateDepartMentAsync(_mapper.Map<DepartMentStateRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    dialog.Success("提示", data.Message);
                    //刷新树
                    DepartMentTreeLoad();
                    if (type == "IsDelete")
                    {
                        _selectedTreeNode = new DepartMentTreeItemDto();
                        DepartMentSelected = new DepartMentInfoDto();
                        ImageClear();
                    }
                    else
                    {
                        NodeInfoLoad(_selectedTreeNode);
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