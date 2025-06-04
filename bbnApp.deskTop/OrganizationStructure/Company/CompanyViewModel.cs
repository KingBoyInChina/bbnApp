using AutoMapper;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.DTOs.CommDto;
using bbnApp.GrpcClients;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf.WellKnownTypes;
using Material.Icons;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.OrganizationStructure.Company
{
    partial class CompanyViewModel:BbnPageBase
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
        /// 公司代码服务
        /// </summary>
        private CompanyGrpcService.CompanyGrpcServiceClient _client;
        /// <summary>
        /// 文件上传服务
        /// </summary>
        private UploadFileGrpc.UploadFileGrpcClient _uploadClient;
        /// <summary>
        /// 高德地图服务
        /// </summary>
        private GuideGrpc.GuideGrpcClient _mapClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 公司树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<CompanyTreeItemDto> _companyTreeSource = new ObservableCollection<CompanyTreeItemDto>();
        /// <summary>
        /// 父级公司代码
        /// </summary>
        [ObservableProperty] private ObservableCollection<CompanyItemDto> _companyListSource = new ObservableCollection<CompanyItemDto>();
        /// <summary>
        /// 选中的父级公司代码
        /// </summary>
        [ObservableProperty] private CompanyItemDto _pCompanySelected = new CompanyItemDto {Id="-1",Name="无",Tag=""};
        /// <summary>
        /// 选中的树节点
        /// </summary>
        private CompanyTreeItemDto _selectedTreeNode = new CompanyTreeItemDto();
        /// <summary>
        /// 选中的公司
        /// </summary>
        [ObservableProperty] private CompanyInfoDto _companySelected = new CompanyInfoDto();
        /// <summary>
        /// 查询条件
        /// </summary>
        [ObservableProperty] private string _filterName ="";
        /// <summary>
        /// 1015 公司类型
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _companyTypeList = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _companyTypeListSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 1014	公司级别
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _companyLeveList = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _companyLeveListSelected = new ComboboxItem("", "", "");

        /// <summary>
        /// 选中行政区划代码
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _areaCodeSelected;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public CompanyViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("OrganizationStructure", "公司信息", MaterialIconKind.Company, "",1)
        {
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _client = grpcClientFactory.CreateClient<CompanyGrpcService.CompanyGrpcServiceClient>();
            _mapClient = grpcClientFactory.CreateClient<GuideGrpc.GuideGrpcClient>();
            _uploadClient = grpcClientFactory.CreateClient<UploadFileGrpc.UploadFileGrpcClient>();
            _mapper = mapper;
            this.dialog = dialog;
        }
        /// <summary>
        /// 初始化字典
        /// </summary>
        public async Task CompanyDicInit(UserControl uc)
        {
            NowControl = uc;//当前控件

            AreaCodeSelected = new AreaTreeNodeDto
            {
                AreaId = UserContext.CurrentUser.AreaCode,
                AreaFullName = UserContext.CurrentUser.AreaName,
                AreaName = UserContext.CurrentUser.AreaName
            };

            CompanyLeveList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1014"));
            CompanyTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1015"));
            await GetCompanyItems();
            CompanyTreeLoad();
        }
        /// <summary>
        /// 公司树加载
        /// </summary>
        private void CompanyTreeLoad()
        {
            dialog.ShowLoading("加载公司中...", async e =>
            {
                try
                {
                    CompanyTreeRequestDto request = new CompanyTreeRequestDto
                    {
                        CompanyCode = string.Empty,
                        CompanyName =FilterName,
                        AreaCode =string.Empty
                    };
                    var data = await _client.GetCompanyTreeAsync(_mapper.Map<CompanyTreeRequest>(request), CommAction.GetHeader());
                    if (data.Code)
                    {
                        CompanyTreeSource = new ObservableCollection<CompanyTreeItemDto>(_mapper.Map<List<CompanyTreeItemDto>>(data.Items));
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }

                }
                catch (Exception ex)
                {
                    dialog.Error("加载公司树失败", ex.Message);
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
                var data = await _client.GetCompanyItemsAsync(_mapper.Map<CompanyRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    var list = _mapper.Map<List<CompanyItemDto>>(data.CompanyItems);
                    list.Add(new CompanyItemDto { Id = "-1", Name = "无", Tag = "", Yhid = UserContext.CurrentUser.Yhid });
                    CompanyListSource = new ObservableCollection<CompanyItemDto>(list);
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
        }
        /// <summary>
        /// 公司树刷新
        /// </summary>
        [RelayCommand]
        private void CompanyTreeReload()
        {
            CompanyTreeLoad();
        }
        /// <summary>
        /// 树选中
        /// </summary>
        /// <param name="node"></param>
        public void TreeSelecte(CompanyTreeItemDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if ((bool)node.IsLeaf)
                {
                    if (CompanySelected == null || CompanySelected?.CompanyId != node.Id)
                    {
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
        private void NodeInfoLoad(CompanyTreeItemDto node)
        {
            dialog.ShowLoading("数据读取中...", async e => {
                CompanyInfoRequestDto request = new CompanyInfoRequestDto { CompanyId = node.Id };
                var header = CommAction.GetHeader();
                var data = await _client.GetCompanyInfoAsync(_mapper.Map<CompanyInfoRequest>(request), header);
                dialog.LoadingClose(e);
                if (data.Code)
                {
                    CompanySelected = _mapper.Map<CompanyInfoDto>(data.Item);
                    //设置选中项目
                    SetItemSelected();

                    if (!string.IsNullOrEmpty(CompanySelected.CompanyId))
                    {
                        await FileRead(CompanySelected.CompanyId);
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
            CompanyTypeListSelected = CommAction.SetSelectedItem(CompanyTypeList, CompanySelected.CompanyType);
            CompanyLeveListSelected = CommAction.SetSelectedItem(CompanyLeveList, CompanySelected.CompanyLeveName);
            AreaCodeSelected = new AreaTreeNodeDto { 
                AreaFullName = CompanySelected.AreaName,
                AreaId = CompanySelected.AreaCode,
                AreaPoint = CompanySelected.Location
            };
            PCompanySelected= SetCompanySelectedItem(CompanySelected.PCompanyId);
        }
        /// <summary>
        /// 设置父级公司选中项
        /// </summary>
        /// <param name="value"></param>
        private CompanyItemDto SetCompanySelectedItem(string value)
        {
            if (CompanyListSource != null && CompanyListSource.Count > 0)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return CompanyListSource.FirstOrDefault();
                }
                else
                {
                    var item = CompanyListSource.FirstOrDefault(x => x.Id == value || x.Name == value);
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// 显示图片
        /// </summary>
        [ObservableProperty] private Bitmap _companyImage;
        /// <summary>
        /// 是否有图片
        /// </summary>
        [ObservableProperty] private bool _imageShow = false;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private FileItemsDto _companyImageInfo = new FileItemsDto();
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
            if (CompanyImage != null)
            {
                CompanyImage.Dispose();
                CompanyImage = null;
            }
            if (request != null)
            {
                request = null;
            }
            CompanyImageInfo = new FileItemsDto();
        }
        /// <summary>
        /// 文件选择
        /// </summary>
        [RelayCommand]
        private async Task FileSeleced()
        {
            try
            {
                if (CompanySelected != null && !string.IsNullOrEmpty(CompanySelected.CompanyId))
                {
                    var data = await dialog.FileSelected(NowControl, "image");
                    if (data.Item1)
                    {
                        CompanyImage = data.Item4;
                        byte[] imagebytes = data.Item3;
                        var header = CommAction.GetHeader();

                        FileItemsDto items = new FileItemsDto
                        {
                            FileBytes = imagebytes,
                            FileId = CompanyImageInfo.FileId,
                            FileExt = data.Item5.Extension,
                            FileName = data.Item5.Name
                        };

                        UploadFileItemDto fileData = new UploadFileItemDto
                        {
                            ReMarks = (imagebytes.Length / 1024).ToString("0") + "Kb",
                            LinkKey = CompanySelected.CompanyId,
                            LinkTable = "companycode",
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
        private async Task FileRead(string linkKey, string linkTable = "companycode")
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
                        CompanyImageInfo = item.Files[0];
                        CompanyImage = CommAction.ByteArrayToBitmap(CompanyImageInfo.FileBytes);
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
        /// 新建公司
        /// </summary>
        [RelayCommand]
        private async Task AddCompany()
        {
            CompanySelected = new CompanyInfoDto();
            await Task.Delay(200);

            CompanySelected.IdxNum = 0;
            SetItemSelected();
            ImageClear();
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private void CompanySave()
        {
            try
            {
                StringBuilder _error = new StringBuilder();
                CompanySelected.CompanyType = CompanyTypeListSelected?.Id;
                CompanySelected.PCompanyId=PCompanySelected?.Id;
                CompanySelected.AreaName = AreaCodeSelected?.AreaFullName;
                CompanySelected.AreaCode = AreaCodeSelected?.AreaId;
                CompanySelected.Location = AreaCodeSelected?.AreaPoint;
                if (string.IsNullOrEmpty(CompanySelected.CompanyName))
                {
                    _error.AppendLine($"公司名称不能为空");
                }
                if (string.IsNullOrEmpty(CompanySelected.CompanyType))
                {
                    _error.AppendLine($"公司类型不能为空");
                }
                if (string.IsNullOrEmpty(CompanySelected.AreaCode))
                {
                    _error.AppendLine($"公司所在地区不能为空");
                }
                if (string.IsNullOrEmpty(CompanySelected.CompanyLeveName))
                {
                    _error.AppendLine($"公司组织机构级别不能为空");
                }
                if (string.IsNullOrEmpty(CompanySelected.OrganizationCode))
                {
                    _error.AppendLine($"公司组织机构代码不能为空");
                }
                if (!string.IsNullOrEmpty(_error.ToString()))
                {
                    dialog.Error("提示", _error.ToString());
                    return;
                }

                dialog.ShowLoading("数据提交中...", async e => {
                    try
                    {
                        CompanySaveRequestDto request = new CompanySaveRequestDto
                        {
                            Item = CompanySelected
                        };
                        var header = CommAction.GetHeader();
                        var data = await _client.SaveCompanyInfoAsync(_mapper.Map<CompanySaveRequest>(request), header);

                        if (data.Code)
                        {
                            dialog.Success("提示", data.Message);

                            CompanySelected = _mapper.Map<CompanyInfoDto>(data.Item);
                            ImageShow = true;
                            CompanyTreeLoad();
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
        public async Task CompanyState(string type, CompanyTreeItemDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if (type == "IsDelete")
                {
                    CompanyStatePost(type, node.Id, "");
                }
                else
                {
                    if ( !(bool) node.IsLock)
                    {
                        var data = await dialog.Prompt("请输入锁定原因", "确定");
                        if (data.Item1)
                        {
                            CompanyStatePost(type, node.Id, data.Item2);
                        }
                    }
                    else
                    {
                        CompanyStatePost(type, node.Id, "");
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
        private void CompanyStatePost(string type, string Id, string Reason)
        {
            dialog.ShowLoading("数据处理中...", async e => {
                CompanyStateRequestDto request = new CompanyStateRequestDto
                {
                    Type = type,
                    CompanyId = Id,
                    Reason=Reason
                };
                var data = await _client.StateCompanyInfoAsync(_mapper.Map<CompanyStateRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    dialog.Success("提示", data.Message);
                    //刷新树
                    CompanyTreeLoad();
                    if (type == "IsDelete")
                    {
                        _selectedTreeNode = new CompanyTreeItemDto();
                        CompanySelected = new CompanyInfoDto();
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
