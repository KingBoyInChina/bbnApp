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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Material.Icons;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.PlatformManagement.MaterialsCode
{
    partial class MaterialsCodeViewModel : BbnPageBase
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
        /// 查询状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 
        /// </summary>
        private MaterialsCodeGrpc.MaterialsCodeGrpcClient _client;
        private UploadFileGrpc.UploadFileGrpcClient _uploadClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 资产树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<MaterialTreeItemDto> _materialTreeSource = new ObservableCollection<MaterialTreeItemDto>();
        /// <summary>
        /// 选中的树节点
        /// </summary>
        private MaterialTreeItemDto _selectedTreeNode;
        /// <summary>
        /// 选中的字典
        /// </summary>
        [ObservableProperty] private MaterialsCodeDto _materialSelected;
        /// <summary>
        /// 查询条件
        /// </summary>
        [ObservableProperty] private string _filterName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public MaterialsCodeViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("PlatformManagement", "物资耗材代码", MaterialIconKind.Material, "", 4)
        {
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _client = grpcClientFactory.CreateClient<MaterialsCodeGrpc.MaterialsCodeGrpcClient>().GetAwaiter().GetResult();
            _uploadClient = grpcClientFactory.CreateClient<UploadFileGrpc.UploadFileGrpcClient>().GetAwaiter().GetResult();
            _mapper = mapper;
            this.dialog = dialog;
        }

        /// <summary>
        /// 1209	计量单位
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _materialUnit = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _materialUnitSelected = new ComboboxItem("", "", "");
        /// 1210	物资类型
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _materialType = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _materialTypeSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 1211	形态类型
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _materialForm = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _materialFormSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 1212	材质
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _materialSupplies = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _materialSuppliesSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 1213	危险物资分类
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _materialDangerType = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _materialDangerTypeSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 初始化字典
        /// </summary>
        public void MaterialDicInit(UserControl uc)
        {
            NowControl = uc;//当前控件

            MaterialUnit = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1209"));
            MaterialType = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1210"));
            MaterialForm = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1211"));
            MaterialSupplies = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1212"));
            MaterialDangerType = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1213"));
            MaterialTreeLoad();
        }
        /// <summary>
        /// 树加载
        /// </summary>
        [RelayCommand]
        private void MaterialTree()
        {
            MaterialTreeLoad();
        }
        /// <summary>
        /// 加载树
        /// </summary>
        private void MaterialTreeLoad()
        {
            try
            {
                dialog.ShowLoading("物资数据加载中", async (e) =>
                {
                    MaterialsCodeTreeRequestDto request = new MaterialsCodeTreeRequestDto { FilterKey = FilterName };
                    var header = CommAction.GetHeader();
                    var data =await _client.GetMaterailTreeAsync(_mapper.Map<MaterialsCodeTreeRequest>(request), header);
                    if (data.Code)
                    {
                        dialog.Success("提示", data.Message);
                        MaterialTreeSource = [.. _mapper.Map<List<MaterialTreeItemDto>>(data.Items)];
                        dialog.LoadingClose(e);
                    }
                    else
                    {
                        dialog.Error("错误提示", data.Message);
                    }
                    dialog.LoadingClose(e);
                });
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }

        /// <summary>
        /// 树选中
        /// </summary>
        /// <param name="node"></param>
        public async Task TreeSelecte(MaterialTreeItemDto node)
        {
            try
            {
                ImageClear();
                _selectedTreeNode = node;
                if (node.IsLeaf)
                {
                   await NodeInfoLoad(node);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 材料信息
        /// </summary>
        private async Task NodeInfoLoad(MaterialTreeItemDto node)
        {
            MaterialsCodeInfoRequestDto request = new MaterialsCodeInfoRequestDto { MaterialId = node.Id };
            var header = CommAction.GetHeader();
            var data =await _client.GetMaterialInfoAsync(_mapper.Map<MaterialsCodeInfoRequest>(request), header);
            if (data.Code)
            {
                MaterialSelected = _mapper.Map<MaterialsCodeDto>(data.Item);
                //设置选中项目
                SetItemSelected();
                if (!string.IsNullOrEmpty(MaterialSelected.MaterialId))
                {
                    _=FileRead(MaterialSelected.MaterialId);
                }
            }
            else
            {
                dialog.Error("提示", data.Message);
            }
        }
        /// <summary>
        /// 新建材料
        /// </summary>
        [RelayCommand]
        private async Task AddMaterial()
        {
            try
            {
                MaterialSelected = new MaterialsCodeDto();
                await Task.Delay(200);
                
                MaterialSelected.IdxNum = int.MinValue;
                MaterialSelected.MaterialIndex = int.MinValue;
                MaterialSelected.IsDanger = 0;
                SetItemSelected();
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 设置选中项目
        /// </summary>
        private void SetItemSelected()
        {
            MaterialTypeSelected = CommAction.SetSelectedItem(MaterialType, MaterialSelected.MaterialType);
            MaterialUnitSelected = CommAction.SetSelectedItem(MaterialUnit, MaterialSelected.Unit);
            MaterialFormSelected = CommAction.SetSelectedItem(MaterialForm, MaterialSelected.MaterialForm);
            MaterialSuppliesSelected = CommAction.SetSelectedItem(MaterialSupplies, MaterialSelected.MaterialSupplies);
        }
        /// <summary>
        /// 材料信息提交
        /// </summary>
        [RelayCommand]
        private void MaterialSave()
        {
            try
            {
                string msg = string.Empty;
                IsBusy = true;
                #region 值处理
                MaterialSelected.MaterialType= MaterialTypeSelected?.Id;
                MaterialSelected.MaterialForm = MaterialFormSelected?.Id;
                MaterialSelected.MaterialSupplies = MaterialSuppliesSelected?.Id;
                MaterialSelected.DangerType = MaterialDangerTypeSelected?.Id;
                MaterialSelected.Unit = MaterialUnitSelected?.Id;
                #endregion
                #region 逻辑
                if (string.IsNullOrEmpty(MaterialSelected.MaterialName))
                {
                    msg = "物资名称不能空";
                }
                else if (string.IsNullOrEmpty(MaterialSelected.MaterialType))
                {
                    msg = "物资分类不能空";
                }
                else if (string.IsNullOrEmpty(MaterialSelected.MaterialBarCode))
                {
                    msg = "物资条码不能空";
                }
                else if (string.IsNullOrEmpty(MaterialSelected.Unit))
                {
                    msg = "物资计量单位不能空";
                }
                else if (string.IsNullOrEmpty(MaterialSelected.MaterialForm))
                {
                    msg = "物资形态不能空";
                }
                else if (string.IsNullOrEmpty(MaterialSelected.MaterialSupplies))
                {
                    msg = "物资材质不能空";
                }
                else if (MaterialSelected.IsDanger==1&& string.IsNullOrEmpty(MaterialSelected.DangerType))
                {
                    msg = "危险物资危险类型不能空";
                }
                else if (string.IsNullOrEmpty(MaterialSelected.StorageEnvironment))
                {
                    msg = "物资存储环境不能空";
                }
                #endregion
                if (string.IsNullOrEmpty(msg))
                {
                    dialog.ShowLoading("数据提交中...",async (e) => {
                        MaterialsCodeSaveRequestDto request = new MaterialsCodeSaveRequestDto { 
                            Item=MaterialSelected
                        };
                        var header = CommAction.GetHeader();
                        var data =await _client.MaterialSaveAsync(_mapper.Map<MaterialsCodeSaveRequest>(request), header);
                        dialog.LoadingClose(e);
                        IsBusy = false;
                        if (data.Code)
                        {
                            dialog.Success("提示",data.Message);
                            MaterialSelected = _mapper.Map<MaterialsCodeDto>(data.Item);
                            ImageShow = true;
                            MaterialTreeLoad();
                        }
                        else
                        {
                            dialog.Error("提示", data.Message);
                        }
                    });

                }
                else
                {
                    IsBusy = false;
                    dialog.Error("提示",msg);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="model"></param>
        public async Task MaterialState(string type, MaterialTreeItemDto model)
        {
            var data = new MaterialsCodeStateRequestDto { MaterialId=model.Id,Type=type};
            if (type == "IsLock")
            {
                #region 锁定
                await IsLockState(model.IsLock,data);
                #endregion
            }
            else if (type == "IsDelete")
            {
                #region 删除
                var b=await dialog.Confirm("提示","确定删除当前物资代码吗？","删除","取消");
                if (b)
                {
                     await MaterialStateSave(data);
                    ImageClear();
                }
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsLock"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task IsLockState(bool IsLock, MaterialsCodeStateRequestDto model)
        {
            if (!IsLock)
            {
                var data=await dialog.Prompt("提示", "请填写锁定原因？");
                if (data.Item1)
                {
                    model.LockReason = data.Item2;
                    await MaterialStateSave(model);
                }
            }
            else
            {
                await MaterialStateSave(model);
            }
        }
        /// <summary>
        /// 状态变更
        /// </summary>
        private async Task MaterialStateSave(MaterialsCodeStateRequestDto request)
        {
            try
            {
                var header = CommAction.GetHeader();
                var data = await _client.MaterialStateAsync(_mapper.Map<MaterialsCodeStateRequest>(request), header);
                if (data.Code)
                {
                    dialog.Success("提示", data.Message);
                    MaterialSelected =_mapper.Map<MaterialsCodeDto>(data.Item);
                    MaterialTreeLoad();
                }
                else
                {

                    dialog.Error("提示", data.Message);
                }
            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 显示图片
        /// </summary>
        [ObservableProperty] private Bitmap _materialImage;
        /// <summary>
        /// 是否有图片
        /// </summary>
        [ObservableProperty] private bool _imageShow = false;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private FileItemsDto _materialImageInfo = new FileItemsDto();
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
            if (MaterialImage != null)
            {
                MaterialImage.Dispose();
                MaterialImage = null;
            }
            if (request != null)
            {
                request = null;
            }
            MaterialImageInfo = new FileItemsDto();
        }
        /// <summary>
        /// 文件选择
        /// </summary>
        [RelayCommand]
        private async Task FileSeleced()
        {
            try
            {
                if (MaterialSelected != null&&!string.IsNullOrEmpty(MaterialSelected.MaterialId)) { 
                var data =await dialog.FileSelected(NowControl, "image");
                    if (data.Item1)
                    {
                        MaterialImage = data.Item4;
                        byte[] imagebytes = data.Item3;
                        var header = CommAction.GetHeader();

                        FileItemsDto items = new FileItemsDto
                        {
                            FileBytes = imagebytes,
                            FileId = MaterialImageInfo.FileId,
                            FileExt = data.Item5.Extension,
                            FileName = data.Item5.Name
                        };

                        UploadFileItemDto fileData = new UploadFileItemDto
                        {
                            ReMarks = (imagebytes.Length/1024).ToString("0")+"Kb",
                            LinkKey = MaterialSelected.MaterialId,
                            LinkTable = "materialscode",
                            Files =new List<FileItemsDto> { items }
                        };

                        request = new UploadFileRequestDto
                        {
                            Item = fileData
                        };
                        bool post=await dialog.Confirm("提示", "是否上传当前文件？", "上传", "取消");
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
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
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
        private async Task FileRead(string linkKey,string linkTable= "materialscode")
        {
            try
            {
                var header = CommAction.GetHeader();
                UploadFileReadRequestDto request = new UploadFileReadRequestDto
                {
                    LinkKey = linkKey,
                    LinkTable = linkTable
                };

                var response = await _uploadClient.UploadFileReadAsync(_mapper.Map<UploadFileReadRequest>(request), header);
                if (response.Code)
                {
                    UploadFileItemDto item=_mapper.Map<UploadFileItemDto>(response.Item);
                    if (item != null)
                    {
                        MaterialImageInfo = item.Files[0];
                        MaterialImage =CommAction.ByteArrayToBitmap(MaterialImageInfo.FileBytes);
                    }
                }
                ImageShow = true;
            }
            catch(Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
    }
}
