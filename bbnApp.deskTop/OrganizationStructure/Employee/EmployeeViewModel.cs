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
        /// ��ǰ�ؼ�
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
        /// ͼƬ����״̬
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// ��Ա�������
        /// </summary>
        private EmployeeGrpc.EmployeeGrpcClient _client;
        /// <summary>
        /// ���Ŵ������
        /// </summary>
        private DepartMentGrpc.DepartMentGrpcClient _departMentClient;
        /// <summary>
        /// ��˾�������
        /// </summary>
        private CompanyGrpcService.CompanyGrpcServiceClient _companyClient;
        /// <summary>
        /// �ļ��ϴ�����
        /// </summary>
        private UploadFileGrpc.UploadFileGrpcClient _uploadClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// ��Ա������Դ
        /// </summary>
        [ObservableProperty] private ObservableCollection<EmployeeTreeItemDto> _employeeTreeSource = new ObservableCollection<EmployeeTreeItemDto>();
        /// <summary>
        /// ����������Դ
        /// </summary>
        [ObservableProperty] private ObservableCollection<DepartMentTreeItemDto> _departMentTreeSource = new ObservableCollection<DepartMentTreeItemDto>();
        /// <summary>
        /// ѡ�еĲ���
        /// </summary>
        [ObservableProperty] private DepartMentTreeItemDto _departMentSelected = new DepartMentTreeItemDto();
        /// <summary>
        /// ���Ŵ����嵥-ԭʼ���ݼ�,���ں���Ҫͨ��ѡ�еĲ��Ź����ϼ�����
        /// </summary>
        [ObservableProperty] private List<DepartMentInfoDto> _departMentList = new List<DepartMentInfoDto>();
        /// <summary>
        /// ���Ŵ����嵥-���ݼ�
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _departMentListSource = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// ��˾�嵥-���ݼ�
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _companyListSource = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// ��˾��Ա�嵥-���ݼ�
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _employeeListSource = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// ѡ�еĹ�˾����
        /// </summary>
        [ObservableProperty] private ComboboxItem _companySelected = new ComboboxItem("", "", "");
        /// <summary>
        /// ѡ�е���Ա
        /// </summary>
        [ObservableProperty] private EmployeeItemDto _employeeSelected = new EmployeeItemDto();
        /// <summary>
        /// ������Ա�嵥
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _departMentEmployeeList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// ��,��������
        /// </summary>
        [ObservableProperty] private ComboboxItem _employeeDepartmentSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 1012 ְλ���ݼ�
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _positionTypeList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// ѡ�е�ְλ
        /// </summary>
        [ObservableProperty] private ComboboxItem _positionSelected = new ComboboxItem("","","");
        /// <summary>
        /// 1002 �Ա����ݼ�
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _genderTypeList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// ѡ�е�ְλ
        /// </summary>
        [ObservableProperty] private ComboboxItem _genderSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// ��������
        /// </summary>
        [ObservableProperty] private DateTimeOffset _birthDateTimeOffset=new DateTimeOffset(DateTime.Now);
        /// <summary>
        /// ��ְ����
        /// </summary>
        [ObservableProperty] private DateTimeOffset _dateOfEmploymentTimeOffset;

        /// <summary>
        /// ��˾ѡ����
        /// </summary>
        /// <param name="value"></param>
        partial void OnCompanySelectedChanged(ComboboxItem value)
        {
            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Id))
                {
                    dialog.ShowLoading("���ݳ�ʼ����...", async e =>
                    {
                        try
                        {
                            //��˾��������
                            DepartMentTreeLoad();
                            await Task.Delay(200);  
                            await GeDepartMentItems();
                            await Task.Delay(200);
                            await EmployeeItemsLoad();
                            //�����Ա��Ϣ
                            DepartMentEmployeeList = new ObservableCollection<ComboboxItem>();
                            EmployeeSelected = new EmployeeItemDto();
                            ImageClear();
                        }
                        catch (Exception ex)
                        {
                            dialog.Error("��ʾ", ex.Message.ToString());
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
        /// ѡ�еĲ��Ŷ���������
        /// </summary>
        /// <param name="value"></param>
        partial void OnDepartMentSelectedChanged(DepartMentTreeItemDto value)
        {
            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Id))
                {
                    //������Ա����
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
        /// �������ű��
        /// </summary>
        /// <param name="item"></param>
        partial void OnEmployeeDepartmentSelectedChanged(ComboboxItem value)
        {
            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Id))
                {
                    //������Ա����
                    DepartMentEmployeeFilter(value.Id);
                }
            }
        }
        /// <summary>
        /// ��,�ֹ��쵼
        /// </summary>
        [ObservableProperty] private ComboboxItem _pEmployeeSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public EmployeeViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("OrganizationStructure", "Ա����Ϣ", MaterialIconKind.Worker, "", 3)
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
        /// ��ʼ���ֵ�
        /// </summary>
        public async Task EmployeeDicInit(UserControl uc)
        {
            NowControl = uc;//��ǰ�ؼ�
            await GetCompanyItems();
            CompanySelected = CommAction.SetSelectedItem(CompanyListSource, UserContext.CurrentUser.CompanyId);
            PositionTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1012"));
            GenderTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1002"));
        }
        /// <summary>
        /// ��˾�嵥����
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
                    dialog.Error("��ʾ", data.Message);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("��ʾ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// ��ȡѡ�й�˾�����嵥
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
                    list.Add(new DepartMentInfoDto { DepartMentId = "-1", DepartMentName = "��", CompanyId =CompanySelected.Id });
                    DepartMentList = list;
                    List<ComboboxItem> items = list.Select(x => new ComboboxItem(x.DepartMentId, x.DepartMentName, x.CompanyId)).ToList();

                    DepartMentListSource = new ObservableCollection<ComboboxItem>(items);
                }
                else
                {
                    dialog.Error("��ʾ", data.Message);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("��ʾ", ex.Message.ToString());
            }
        }

        /// <summary>
        /// ���ع�˾������Ա��Ϣ
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
                    dialog.Error("��ʾ", data.Message);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("��ʾ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// ������ѡ��ʱ-������Ա����
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
            list.Add(new ComboboxItem("-1","��",DepartMentId));
            DepartMentEmployeeList =new ObservableCollection<ComboboxItem>(list);
        }
        /// <summary>
        /// ѡ�й�˾����������
        /// </summary>
        private void DepartMentTreeLoad()
        {
            dialog.ShowLoading("���ز�����...", async e =>
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
                        dialog.Error("��ʾ", data.Message);
                    }

                }
                catch (Exception ex)
                {
                    dialog.Error("���ز�����ʧ��", ex.Message);
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
        /// <summary>
        /// ������ˢ��
        /// </summary>
        [RelayCommand]
        private void DepartMentTreeReload()
        {
            DepartMentTreeLoad();
        }
        /// <summary>
        /// ������ѡ��
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
                dialog.Error("��ʾ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// ��Ա������
        /// </summary>
        [RelayCommand]
        private void EmployeeTreeReload()
        {
            EmployeeTreeLoad();
        }
        /// <summary>
        /// ����ָ�����ŵ�Ա����Ϣ
        /// </summary>
        private void EmployeeTreeLoad()
        {
            dialog.ShowLoading("��Ա��������...",async e =>
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
                        dialog.Error("��ʾ", data.Message);
                    }
                }
                catch (Exception ex)
                {
                    dialog.Error("��ʾ", ex.Message.ToString());
                }
                finally
                {
                    dialog.LoadingClose(e);
                }
            });
        }
        /// <summary>
        /// ��Ա��Ϣ
        /// </summary>
        public void NodeInfoLoad(EmployeeTreeItemDto node)
        {
            dialog.ShowLoading("���ݶ�ȡ��...", async e =>
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
                    dialog.Error("��ʾ", data.Message);
                }
            });

        }
        /// <summary>
        /// ����ѡ����Ŀ
        /// </summary>
        private void SetItemSelected()
        {
            //��������
            EmployeeDepartmentSelected= CommAction.SetSelectedItem(DepartMentListSource, EmployeeSelected.DepartMentId);
            //�ϼ�������
            PEmployeeSelected = CommAction.SetSelectedItem(DepartMentEmployeeList, EmployeeSelected.PEmployeeId);
            //ְ��
            PositionSelected = CommAction.SetSelectedItem(PositionTypeList, EmployeeSelected.Position);
            //�Ա�
            GenderSelected= CommAction.SetSelectedItem(GenderTypeList, EmployeeSelected.Gender);
            //��������
            if (!string.IsNullOrEmpty(EmployeeSelected.BirthDate))
            {
                BirthDateTimeOffset = new DateTimeOffset(Convert.ToDateTime(EmployeeSelected.BirthDate));
            }
            else
            {
                BirthDateTimeOffset= DateTime.Now;
            }
            //��ְ����
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
        /// ��ʾͼƬ
        /// </summary>
        [ObservableProperty] private Bitmap _employeeImage;
        /// <summary>
        /// �Ƿ���ͼƬ
        /// </summary>
        [ObservableProperty] private bool _imageShow = false;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private FileItemsDto _employeeImageInfo = new FileItemsDto();
        /// <summary>
        /// ���ϴ����ļ��������
        /// </summary>
        private UploadFileRequestDto request = null;

        /// <summary>
        /// ͼƬ��Ϣ���
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
        /// �ļ�ѡ��
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
                        bool post = await dialog.Confirm("��ʾ", "�Ƿ��ϴ���ǰ�ļ���", "�ϴ�", "ȡ��");
                        if (post)
                        {
                            FileUploadCommand.Execute(null);
                        }
                    }
                }
                else
                {
                    dialog.Error("��ʾ", "����ѡ��������Ϣ");
                }
            }
            catch (Exception ex)
            {
                dialog.Error("��ʾ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// �ϴ�
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
                        dialog.Success("��ʾ", response.Message);
                        request = null;
                    }
                    else
                    {
                        dialog.Error("��ʾ", response.Message);
                    }
                }
                else
                {
                    dialog.Error("��ʾ", "����ѡ����Ҫ�ϴ����ļ�");
                }
            }
            catch (Exception ex)
            {
                dialog.Error("��ʾ", ex.Message.ToString());
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
                dialog.Error("��ʾ", ex.Message.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        /// <summary>
        /// �½���Ա
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
        /// �����ύ
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
                    _error.AppendLine($"Ա����������Ϊ��");
                }
                if (string.IsNullOrEmpty(EmployeeSelected.PEmployeeId))
                {
                    _error.AppendLine($"�ֹ��쵼��Ϊ��");
                }
                if (string.IsNullOrEmpty(EmployeeSelected.DepartMentId))
                {
                    _error.AppendLine($"�������Ų���Ϊ��");
                }
                if (string.IsNullOrEmpty(EmployeeSelected.PhoneNum))
                {
                    _error.AppendLine($"��ϵ�绰����Ϊ��");
                }
                if (string.IsNullOrEmpty(EmployeeSelected.DateOfEmployment))
                {
                    _error.AppendLine($"��ְʱ�䲻��Ϊ��");
                }
                if (!string.IsNullOrEmpty(_error.ToString()))
                {
                    dialog.Error("��ʾ", _error.ToString());
                    return;
                }

                dialog.ShowLoading("�����ύ��...", async e =>
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
                            dialog.Success("��ʾ", data.Message);

                            EmployeeSelected = _mapper.Map<EmployeeItemDto>(data.Item);
                            ImageShow = true;
                            if (b)
                            {
                                EmployeeTreeLoad();
                            }
                        }
                        else
                        {
                            dialog.Error("��ʾ", data.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        dialog.Error("��ʾ", ex.Message.ToString());
                    }
                    finally
                    {
                        dialog.LoadingClose(e);
                    }
                });

            }
            catch (Exception ex)
            {
                dialog.Error("��ʾ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// Ա��״̬���
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
                        var data = await dialog.Prompt("����������ԭ��", "ȷ��");
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
                dialog.Error("��ʾ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// Ա��״̬�������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Id"></param>
        /// <param name="Reason"></param>
        private void EmployeeStatePost(string type, string Id, string Reason)
        {
            dialog.ShowLoading("���ݴ�����...", async e =>
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
                    dialog.Success("��ʾ", data.Message);
                    //ˢ����
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
                    dialog.Error("��ʾ", data.Message);
                }
            });
        }
    }
}