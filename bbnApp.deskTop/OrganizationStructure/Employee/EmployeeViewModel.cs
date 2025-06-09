using AutoMapper;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.BusinessDto;
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

namespace bbnApp.deskTop.OrganizationStructure.Employee
{
    partial class EmployeeViewModel : BbnPageBase
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
        /// 人员代码服务
        /// </summary>
        private EmployeeGrpc.EmployeeGrpcClient _client;
        /// <summary>
        /// 部门代码服务
        /// </summary>
        private DepartMentGrpc.DepartMentGrpcClient _departMentClient;
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
        /// 人员树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<EmployeeTreeItemDto> _employeeTreeSource = new ObservableCollection<EmployeeTreeItemDto>();
        /// <summary>
        /// 部门树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<DepartMentTreeItemDto> _departMentTreeSource = new ObservableCollection<DepartMentTreeItemDto>();
        /// <summary>
        /// 选中的部门
        /// </summary>
        [ObservableProperty] private DepartMentTreeItemDto _departMentSelected = new DepartMentTreeItemDto();
        /// <summary>
        /// 部门代码清单-原始数据集,用于后需要通过选中的部门过滤上级部门
        /// </summary>
        [ObservableProperty] private List<DepartMentInfoDto> _departMentList = new List<DepartMentInfoDto>();
        /// <summary>
        /// 部门代码清单-数据集
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _departMentListSource = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 公司清单-数据集
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _companyListSource = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 公司人员清单-数据集
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _employeeListSource = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的公司代码
        /// </summary>
        [ObservableProperty] private ComboboxItem _companySelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 选中的人员
        /// </summary>
        [ObservableProperty] private EmployeeItemDto _employeeSelected = new EmployeeItemDto();
        /// <summary>
        /// 部门人员清单
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _departMentEmployeeList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 表单,所属部门
        /// </summary>
        [ObservableProperty] private ComboboxItem _employeeDepartmentSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 1012 职位数据集
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _positionTypeList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的职位
        /// </summary>
        [ObservableProperty] private ComboboxItem _positionSelected = new ComboboxItem("","","");
        /// <summary>
        /// 1002 性别数据集
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _genderTypeList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的职位
        /// </summary>
        [ObservableProperty] private ComboboxItem _genderSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 出身日期
        /// </summary>
        [ObservableProperty] private DateTimeOffset _birthDateTimeOffset=new DateTimeOffset(DateTime.Now);
        /// <summary>
        /// 入职日期
        /// </summary>
        [ObservableProperty] private DateTimeOffset _dateOfEmploymentTimeOffset;

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
                            await GeDepartMentItems();
                            await Task.Delay(200);
                            await EmployeeItemsLoad();
                            //清空人员信息
                            DepartMentEmployeeList = new ObservableCollection<ComboboxItem>();
                            EmployeeSelected = new EmployeeItemDto();
                            ImageClear();
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
        /// 选中的部门对象变更触发
        /// </summary>
        /// <param name="value"></param>
        partial void OnDepartMentSelectedChanged(DepartMentTreeItemDto value)
        {
            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Id))
                {
                    //部门人员重载
                    EmployeeTreeLoad();
                    DepartMentSelected = value;
                    EmployeeSelected = new EmployeeItemDto();
                    BirthDateTimeOffset = new DateTimeOffset();
                    DateOfEmploymentTimeOffset = new DateTimeOffset();
                    ImageClear();
                }
            }
        }
        /// <summary>
        /// 所属部门变更
        /// </summary>
        /// <param name="item"></param>
        partial void OnEmployeeDepartmentSelectedChanged(ComboboxItem value)
        {
            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Id))
                {
                    //部门人员重载
                    DepartMentEmployeeFilter(value.Id);
                }
            }
        }
        /// <summary>
        /// 表单,分管领导
        /// </summary>
        [ObservableProperty] private ComboboxItem _pEmployeeSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public EmployeeViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("OrganizationStructure", "员工信息", MaterialIconKind.Worker, "", 3)
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
            _client =await grpcClientFactory.CreateClient<EmployeeGrpc.EmployeeGrpcClient>();
            _departMentClient = await grpcClientFactory.CreateClient<DepartMentGrpc.DepartMentGrpcClient>();
            _companyClient = await grpcClientFactory.CreateClient<CompanyGrpcService.CompanyGrpcServiceClient>();
            _uploadClient = await grpcClientFactory.CreateClient<UploadFileGrpc.UploadFileGrpcClient>();
        }
        /// <summary>
        /// 初始化字典
        /// </summary>
        public async Task EmployeeDicInit(UserControl uc)
        {
            NowControl = uc;//当前控件
            await GetCompanyItems();
            CompanySelected = CommAction.SetSelectedItem(CompanyListSource, UserContext.CurrentUser.CompanyId);
            PositionTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1012"));
            GenderTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1002"));
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
        /// 获取选中公司部门清单
        /// </summary>
        /// <returns></returns>
        private async Task GeDepartMentItems()
        {
            try
            {
                DepartMentSearchRequestDto request = new DepartMentSearchRequestDto
                {
                    CompanyId = CompanySelected.Id,
                    DepartMentName = string.Empty
                };
                var data = await _departMentClient.GetDepartMentListAsync(_mapper.Map<DepartMentSearchRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    List<DepartMentInfoDto> list = _mapper.Map<List<DepartMentInfoDto>>(data.Items);
                    list.Add(new DepartMentInfoDto { DepartMentId = "-1", DepartMentName = "无", CompanyId =CompanySelected.Id });
                    DepartMentList = list;
                    List<ComboboxItem> items = list.Select(x => new ComboboxItem(x.DepartMentId, x.DepartMentName, x.CompanyId)).ToList();

                    DepartMentListSource = new ObservableCollection<ComboboxItem>(items);
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
        /// 加载公司所有人员信息
        /// </summary>
        private async Task EmployeeItemsLoad()
        {
            try
            {
                EmployeeItemsRequestDto request = new EmployeeItemsRequestDto
                {
                    CompanyId = CompanySelected.Id,
                    DepartMentId = string.Empty,
                    EmployeeName = string.Empty
                };
                var data = await _client.EmployeeListLoadAsync(_mapper.Map<EmployeeItemsRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    var items = data.Items.Select(x => new ComboboxItem(x.EmployeeId, x.EmployeeName, x.DepartMentId)).ToList();
                    EmployeeListSource = new ObservableCollection<ComboboxItem>(items);
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
        /// 表单部门选择时-部门人员过滤
        /// </summary>
        /// <param name="DepartMentId"></param>
        public void DepartMentEmployeeFilter(string DepartMentId)
        {
            string PDepartMentId = string.Empty;
            var departmentItem= DepartMentList.FirstOrDefault(x => x.DepartMentId == DepartMentId);
            if (departmentItem != null)
            {
                PDepartMentId = departmentItem.PDepartMentId;
            }

            var list = EmployeeListSource.Where(x => x.Tag == DepartMentId|| x.Tag == PDepartMentId).OrderBy(x=>x.Tag).ToList();
            if (list.Count == 0)
            {
                list = EmployeeListSource.ToList();
            }
            list.Add(new ComboboxItem("-1","无",DepartMentId));
            DepartMentEmployeeList =new ObservableCollection<ComboboxItem>(list);
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
                DepartMentSelected = node;
                if ((bool)node.IsLeaf)
                {
                    if (DepartMentSelected == null || DepartMentSelected?.Id != node.Id)
                    {
                        ImageClear();
                        EmployeeTreeLoad();
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
        private void EmployeeTreeReload()
        {
            EmployeeTreeLoad();
        }
        /// <summary>
        /// 加载指定部门的员工信息
        /// </summary>
        private void EmployeeTreeLoad()
        {
            dialog.ShowLoading("人员树加载中...",async e =>
            {

                try
                {
                    EmployeeTreeSource.Clear();
                    EmployeeTreeRequestDto request = new EmployeeTreeRequestDto
                    {
                        DepartMentId = DepartMentSelected.Id,
                        CompanyId = DepartMentSelected.Tag,
                        EmployeeName = string.Empty
                    };
                    var data = await _client.EmployeeTreeLoadAsync(_mapper.Map<EmployeeTreeRequest>(request), CommAction.GetHeader());
                    if (data.Code)
                    {
                        EmployeeTreeSource = new ObservableCollection<EmployeeTreeItemDto>(_mapper.Map<List<EmployeeTreeItemDto>>(data.Items));
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
        /// 人员信息
        /// </summary>
        public void NodeInfoLoad(EmployeeTreeItemDto node)
        {
            dialog.ShowLoading("数据读取中...", async e =>
            {
                EmployeeInfoRequestDto request = new EmployeeInfoRequestDto { EmployeeId = node.Id,CompanyId=CompanySelected.Id,CompanyName=CompanySelected.Name };
                var header = CommAction.GetHeader();
                var data = await _client.EmployeeInfoLoadAsync(_mapper.Map<EmployeeInfoRequest>(request), header);
                dialog.LoadingClose(e);
                if (data.Code)
                {
                    EmployeeSelected = _mapper.Map<EmployeeItemDto>(data.Item);
                    SetItemSelected();
                    if (!string.IsNullOrEmpty(EmployeeSelected.EmployeeId))
                    {
                        await FileRead(EmployeeSelected.EmployeeId);
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
            //所属部门
            EmployeeDepartmentSelected= CommAction.SetSelectedItem(DepartMentListSource, EmployeeSelected.DepartMentId);
            //上级管理人
            PEmployeeSelected = CommAction.SetSelectedItem(DepartMentEmployeeList, EmployeeSelected.PEmployeeId);
            //职务
            PositionSelected = CommAction.SetSelectedItem(PositionTypeList, EmployeeSelected.Position);
            //性别
            GenderSelected= CommAction.SetSelectedItem(GenderTypeList, EmployeeSelected.Gender);
            //出生日期
            if (!string.IsNullOrEmpty(EmployeeSelected.BirthDate))
            {
                BirthDateTimeOffset = new DateTimeOffset(Convert.ToDateTime(EmployeeSelected.BirthDate));
            }
            else
            {
                BirthDateTimeOffset= DateTime.Now;
            }
            //入职日期
            if (!string.IsNullOrEmpty(EmployeeSelected.DateOfEmployment))
            {
                DateOfEmploymentTimeOffset = new DateTimeOffset(Convert.ToDateTime(EmployeeSelected.DateOfEmployment));
            }
            else
            {
                DateOfEmploymentTimeOffset = DateTime.Now;
            }
        }
        /// <summary>
        /// 显示图片
        /// </summary>
        [ObservableProperty] private Bitmap _employeeImage;
        /// <summary>
        /// 是否有图片
        /// </summary>
        [ObservableProperty] private bool _imageShow = false;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private FileItemsDto _employeeImageInfo = new FileItemsDto();
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
            if (EmployeeImage != null)
            {
                EmployeeImage.Dispose();
                EmployeeImage = null;
            }
            if (request != null)
            {
                request = null;
            }
            EmployeeImageInfo = new FileItemsDto();
        }
        /// <summary>
        /// 文件选择
        /// </summary>
        [RelayCommand]
        private async Task FileSeleced()
        {
            try
            {
                if (EmployeeSelected != null && !string.IsNullOrEmpty(EmployeeSelected.EmployeeId))
                {
                    var data = await dialog.FileSelected(NowControl, "image");
                    if (data.Item1)
                    {
                        EmployeeImage = data.Item4;
                        byte[] imagebytes = data.Item3;
                        var header = CommAction.GetHeader();

                        FileItemsDto items = new FileItemsDto
                        {
                            FileBytes = imagebytes,
                            FileId = EmployeeImageInfo.FileId,
                            FileExt = data.Item5.Extension,
                            FileName = data.Item5.Name
                        };

                        UploadFileItemDto fileData = new UploadFileItemDto
                        {
                            ReMarks = (imagebytes.Length / 1024).ToString("0") + "Kb",
                            LinkKey = EmployeeSelected.EmployeeId,
                            LinkTable = "employees",
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
        private async Task FileRead(string linkKey, string linkTable = "employees")
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
                        EmployeeImageInfo = item.Files[0];
                        EmployeeImage = CommAction.ByteArrayToBitmap(EmployeeImageInfo.FileBytes);
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
        /// 新建人员
        /// </summary>
        [RelayCommand]
        private async Task AddEmployee()
        {
            EmployeeSelected=new EmployeeItemDto();
            await Task.Delay(200);

            EmployeeSelected.IdxNum = 0;
            EmployeeSelected.PEmployeeId = "-1";
            EmployeeSelected.CompanyId = CompanySelected.Id;
            EmployeeSelected.CompanyName = CompanySelected.Name;
            EmployeeSelected.DepartMentId = DepartMentSelected.Id;
            EmployeeSelected.DepartMentName = DepartMentSelected.Name;
            ImageClear();
            await Task.Delay(200);
            SetItemSelected();
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private void EmployeeSave()
        {
            try
            {
                StringBuilder _error = new StringBuilder();
                EmployeeSelected.Gender = GenderSelected.Name;
                EmployeeSelected.DepartMentName = EmployeeDepartmentSelected.Name;
                EmployeeSelected.DepartMentId = EmployeeDepartmentSelected.Id;
                EmployeeSelected.PEmployeeId = PEmployeeSelected.Id;
                EmployeeSelected.Position = PositionSelected.Id;
                EmployeeSelected.BirthDate=BirthDateTimeOffset.ToString("yyyy-MM-dd");
                EmployeeSelected.DateOfEmployment = DateOfEmploymentTimeOffset.ToString("yyyy-MM-dd");
                if (string.IsNullOrEmpty(EmployeeSelected.EmployeeName))
                {
                    _error.AppendLine($"员工姓名不能为空");
                }
                if (string.IsNullOrEmpty(EmployeeSelected.PEmployeeId))
                {
                    _error.AppendLine($"分管领导能为空");
                }
                if (string.IsNullOrEmpty(EmployeeSelected.DepartMentId))
                {
                    _error.AppendLine($"所属部门不能为空");
                }
                if (string.IsNullOrEmpty(EmployeeSelected.PhoneNum))
                {
                    _error.AppendLine($"联系电话不能为空");
                }
                if (string.IsNullOrEmpty(EmployeeSelected.DateOfEmployment))
                {
                    _error.AppendLine($"入职时间不能为空");
                }
                if (!string.IsNullOrEmpty(_error.ToString()))
                {
                    dialog.Error("提示", _error.ToString());
                    return;
                }

                dialog.ShowLoading("数据提交中...", async e =>
                {
                    try
                    {
                        bool b = false;
                        if (string.IsNullOrEmpty(EmployeeSelected.EmployeeId))
                        {
                            b = true;
                        }
                        EmployeeSaveRequestDto request = new EmployeeSaveRequestDto
                        {
                            Item = EmployeeSelected
                        };
                        var header = CommAction.GetHeader();
                        var data = await _client.EmployeePostAsync(_mapper.Map<EmployeeSaveRequest>(request), header);

                        if (data.Code)
                        {
                            dialog.Success("提示", data.Message);

                            EmployeeSelected = _mapper.Map<EmployeeItemDto>(data.Item);
                            ImageShow = true;
                            if (b)
                            {
                                EmployeeTreeLoad();
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
        /// 员工状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        public async Task DepartMentState(string type, EmployeeTreeItemDto node)
        {
            try
            {
                if (type == "IsDelete")
                {
                    EmployeeStatePost(type, node.Id, "");
                }
                else
                {
                    if (!(bool)node.IsLock)
                    {
                        var data = await dialog.Prompt("请输入锁定原因", "确定");
                        if (data.Item1)
                        {
                            EmployeeStatePost(type, node.Id, data.Item2);
                        }
                    }
                    else
                    {
                        EmployeeStatePost(type, node.Id, "");
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 员工状态变更请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Id"></param>
        /// <param name="Reason"></param>
        private void EmployeeStatePost(string type, string Id, string Reason)
        {
            dialog.ShowLoading("数据处理中...", async e =>
            {
                EmployeeStateRequestDto request = new EmployeeStateRequestDto
                {
                    Type = type,
                    EmployeeId = Id,
                    Reason = Reason
                };
                var data = await _client.EmployeeStateAsync(_mapper.Map<EmployeeStateRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    dialog.Success("提示", data.Message);
                    //刷新树
                    DepartMentTreeLoad();
                    if (type == "IsDelete")
                    {
                        EmployeeSelected = new EmployeeItemDto();
                        ImageClear();
                    }
                    else
                    {
                        NodeInfoLoad(new EmployeeTreeItemDto { Id=EmployeeSelected.EmployeeId,Name=EmployeeSelected.EmployeeName,Tag=EmployeeSelected.DepartMentId});
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