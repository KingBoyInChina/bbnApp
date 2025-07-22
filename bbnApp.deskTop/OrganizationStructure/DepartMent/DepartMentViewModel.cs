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
        /// ���Ŵ������
        /// </summary>
        private DepartMentGrpc.DepartMentGrpcClient _client;
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
        /// ����������Դ
        /// </summary>
        [ObservableProperty] private ObservableCollection<DepartMentTreeItemDto> _departMentTreeSource = new ObservableCollection<DepartMentTreeItemDto>();
        /// <summary>
        /// ������˾����
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _companyListSource = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// ѡ�еĹ�˾����
        /// </summary>
        [ObservableProperty] private ComboboxItem _companySelected = new ComboboxItem("","","");


        /// <summary>
        /// �Զ����ɵľֲ���������������ֵ���ʱ������
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
        /// ѡ�е����ڵ�
        /// </summary>
        private DepartMentTreeItemDto _selectedTreeNode = new DepartMentTreeItemDto();
        /// <summary>
        /// ѡ�еĲ���
        /// </summary>
        [ObservableProperty] private DepartMentInfoDto _departMentSelected = new DepartMentInfoDto();
        /// <summary>
        /// �����嵥
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _departMentList = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// �������������ϼ�����
        /// </summary>
        [ObservableProperty] private ComboboxItem _pDepartMentSelected = new ComboboxItem("","","");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public DepartMentViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("OrganizationStructure", "������Ϣ", MaterialIconKind.OfficeBuilding, "", 2)
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
        /// ��ʼ���ֵ�
        /// </summary>
        public async Task DepartMentDicInit(UserControl uc)
        {
            NowControl = uc;//��ǰ�ؼ�
            await GetCompanyItems();
            await Task.Delay(200);

            CompanySelected = CommAction.SetSelectedItem(CompanyListSource, UserContext.CurrentUser.CompanyId);
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
        /// ��˾�����嵥
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
                    list.Add(new DepartMentInfoDto { DepartMentId="-1",DepartMentName="��",CompanyId= CompanySelected.Id});
                    List<ComboboxItem> items = list.Select(x => new ComboboxItem (x.DepartMentId, x.DepartMentName, x.CompanyId )).ToList();
                    DepartMentList = new ObservableCollection<ComboboxItem>(items);
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
        /// ��˾������
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
                    var data = await _client.GetDepartMentTreeAsync(_mapper.Map<DepartMentTreeRequest>(request), CommAction.GetHeader());
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
        /// ��ѡ��
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
                dialog.Error("��ʾ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// ��˾��Ϣ
        /// </summary>
        private void NodeInfoLoad(DepartMentTreeItemDto node)
        {
            dialog.ShowLoading("���ݶ�ȡ��...", async e => {
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
                    dialog.Error("��ʾ", data.Message);
                }
            });

        }
        /// <summary>
        /// ����ѡ����Ŀ
        /// </summary>
        private void SetItemSelected()
        {
            PDepartMentSelected = CommAction.SetSelectedItem(DepartMentList, DepartMentSelected.PDepartMentId);
        }
        /// <summary>
        /// ��ʾͼƬ
        /// </summary>
        [ObservableProperty] private Bitmap _departMentImage;
        /// <summary>
        /// �Ƿ���ͼƬ
        /// </summary>
        [ObservableProperty] private bool _imageShow = false;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private FileItemsDto _departMentImageInfo = new FileItemsDto();
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
        /// �ļ�ѡ��
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
                dialog.Error("��ʾ", ex.Message.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        /// <summary>
        /// �½�����
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
        /// �����ύ
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
                    _error.AppendLine($"�������Ʋ���Ϊ��");
                }
                if (!string.IsNullOrEmpty(_error.ToString()))
                {
                    dialog.Error("��ʾ", _error.ToString());
                    return;
                }

                dialog.ShowLoading("�����ύ��...", async e => {
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
                            dialog.Success("��ʾ", data.Message);

                            DepartMentSelected = _mapper.Map<DepartMentInfoDto>(data.Item);
                            ImageShow = true;
                            if (b)
                            {
                                DepartMentTreeLoad();
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
        /// ��˾״̬���
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
                        var data = await dialog.Prompt("����������ԭ��", "ȷ��");
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
                dialog.Error("��ʾ", ex.Message.ToString());
            }
        }
        /// <summary>
        /// ��˾״̬�������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Id"></param>
        /// <param name="Reason"></param>
        private void DepartMentStatePost(string type, string Id, string Reason)
        {
            dialog.ShowLoading("���ݴ�����...", async e => {
                DepartMentStateRequestDto request = new DepartMentStateRequestDto
                {
                    Type = type,
                    DepartMentId = Id,
                    Reason = Reason
                };
                var data = await _client.StateDepartMentAsync(_mapper.Map<DepartMentStateRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    dialog.Success("��ʾ", data.Message);
                    //ˢ����
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
                    dialog.Error("��ʾ", data.Message);
                }
            });
        }
    }
}