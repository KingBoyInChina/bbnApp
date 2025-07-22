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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Material.Icons;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.PlatformManagement.DeviceCode
{
    partial class DeviceCodeViewModel : BbnPageBase
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
        /// 物资代码服务
        /// </summary>
        private MaterialsCodeGrpc.MaterialsCodeGrpcClient _materialClient;
        /// <summary>
        /// 设备代码服务
        /// </summary>
        private DeviceCodeGrpc.DeviceCodeGrpcClient _client;
        /// <summary>
        /// 上传服务
        /// </summary>
        private UploadFileGrpc.UploadFileGrpcClient _uploadClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 资产树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<DeviceCodeTreeNodeDto> _deviceTreeSource = new ObservableCollection<DeviceCodeTreeNodeDto>();
        /// <summary>
        /// 选中的树节点
        /// </summary>
        private DeviceCodeTreeNodeDto _selectedTreeNode=new DeviceCodeTreeNodeDto();
        /// <summary>
        /// 选中的设备
        /// </summary>
        [ObservableProperty] private DeviceCodeItemDto _deviceCodeSelected=new DeviceCodeItemDto();
        /// <summary>
        /// 设备构成列表
        /// </summary>
        [ObservableProperty] private ObservableCollection<DeviceStructItemDto> _deviceStructList = new ObservableCollection<DeviceStructItemDto>();
        /// <summary>
        /// 临时设备构成对象，添加或编辑时使用
        /// </summary>
        [ObservableProperty] private DeviceStructGridItemDto _tempDeviceStructObj = new DeviceStructGridItemDto();
        /// <summary>
        /// 查询条件
        /// </summary>
        [ObservableProperty] private string _filterName = string.Empty;
        /// <summary>
        /// 1101 设备类型
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _deviceTypeList = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _deviceTypeListSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 1209	计量单位
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _lifeUnitList = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _lifeUnitListSelected = new ComboboxItem("", "", "");
        
        /// <summary>
        /// 物资代码列表
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _materialCodes = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public DeviceCodeViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("PlatformManagement", "设备代码", MaterialIconKind.Devices, "", 5)
        {
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _materialClient = grpcClientFactory.CreateClient<MaterialsCodeGrpc.MaterialsCodeGrpcClient>().GetAwaiter().GetResult();
            _client = grpcClientFactory.CreateClient<DeviceCodeGrpc.DeviceCodeGrpcClient>().GetAwaiter().GetResult();
            _uploadClient = grpcClientFactory.CreateClient<UploadFileGrpc.UploadFileGrpcClient>().GetAwaiter().GetResult();
            _mapper = mapper;
            this.dialog = dialog;
        }

        /// <summary>
        /// 初始化字典
        /// </summary>
        public void MaterialDicInit(UserControl uc)
        {
            NowControl = uc;//当前控件

            LifeUnitList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1209"));
            DeviceTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1101"));
            _=MateriaListLoad();
            DeviceTreeLoad();
        }
        /// <summary>
        /// 物资清单
        /// </summary>
        private async Task MateriaListLoad()
        {
            try
            {
                MaterialsCodeListRequestDto request = new MaterialsCodeListRequestDto { 
                    MaterialType=""
                };
                var data = await _materialClient.GetMaterialListAsync(_mapper.Map<MaterialsCodeListRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    MaterialCodes =new ObservableCollection<ComboboxItem>();
                    foreach(var item in data.Items)
                    {
                        ComboboxItem comboboxItem = new ComboboxItem(item.MaterialId,item.MaterialName,item.ReMarks);
                        MaterialCodes.Add(comboboxItem);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
        /// <summary>
        /// 设备树加载
        /// </summary>
        private void DeviceTreeLoad()
        {
            try
            {
                DeviceCodeTreeRequestDto request = new DeviceCodeTreeRequestDto { 
                    FilterValue=FilterName
                };
                dialog.ShowLoading("加载设备树中...", async e =>
                {
                    var data = await _client.DeviceTreeLoadAsync(_mapper.Map<DeviceCodeTreeRequest>(request),CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        DeviceTreeSource = new ObservableCollection<DeviceCodeTreeNodeDto>(_mapper.Map<List<DeviceCodeTreeNodeDto>>(data.DeviceCodeItems));
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }

                });
            }
            catch(Exception ex)
            {
                dialog.Error("加载设备树失败", ex.Message);
            }
        }
        /// <summary>
        /// 设备树刷新
        /// </summary>
        [RelayCommand]
        private void DeviceTreeReload()
        {
            DeviceTreeLoad();
        }
        /// <summary>
        /// 树选中
        /// </summary>
        /// <param name="node"></param>
        public void TreeSelecte(DeviceCodeTreeNodeDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if (node.IsLeaf)
                {
                    NodeInfoLoad(node);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 设备信息
        /// </summary>
        private void NodeInfoLoad(DeviceCodeTreeNodeDto node)
        {
            dialog.ShowLoading("数据读取中...",async e => {
                DeviceCodeInfoRequestDto request = new DeviceCodeInfoRequestDto { DeviceId = node.Id };
                var header = CommAction.GetHeader();
                var data = await _client.DeviceInfoLoadAsync(_mapper.Map<DeviceCodeInfoRequest>(request), header);
                dialog.LoadingClose(e);
                if (data.Code)
                {
                    DeviceCodeSelected = _mapper.Map<DeviceCodeItemDto>(data.Item);
                    DeviceStructList =new ObservableCollection<DeviceStructItemDto>(_mapper.Map<List<DeviceStructItemDto>>(data.List));
                    //设置选中项目
                    SetItemSelected();
                    if (!string.IsNullOrEmpty(DeviceCodeSelected.DeviceId))
                    {
                        await FileRead(DeviceCodeSelected.DeviceId);
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
            DeviceTypeListSelected = CommAction.SetSelectedItem(DeviceTypeList, DeviceCodeSelected.DeviceType);
            LifeUnitListSelected = CommAction.SetSelectedItem(LifeUnitList, DeviceCodeSelected.LifeUnit);
        }
        /// <summary>
        /// 显示图片
        /// </summary>
        [ObservableProperty] private Bitmap _deviceCodeImage;
        /// <summary>
        /// 是否有图片
        /// </summary>
        [ObservableProperty] private bool _imageShow = false;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private FileItemsDto _deviceCodeImageInfo = new FileItemsDto();
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
            if (DeviceCodeImage != null)
            {
                DeviceCodeImage.Dispose();
                DeviceCodeImage = null;
            }
            if (request != null)
            {
                request = null;
            }
            DeviceCodeImageInfo = new FileItemsDto();
        }
        /// <summary>
        /// 文件选择
        /// </summary>
        [RelayCommand]
        private async Task FileSeleced()
        {
            try
            {
                if (DeviceCodeSelected != null && !string.IsNullOrEmpty(DeviceCodeSelected.DeviceId))
                {
                    var data = await dialog.FileSelected(NowControl, "image");
                    if (data.Item1)
                    {
                        DeviceCodeImage = data.Item4;
                        byte[] imagebytes = data.Item3;
                        var header = CommAction.GetHeader();

                        FileItemsDto items = new FileItemsDto
                        {
                            FileBytes = imagebytes,
                            FileId = DeviceCodeImageInfo.FileId,
                            FileExt = data.Item5.Extension,
                            FileName = data.Item5.Name
                        };

                        UploadFileItemDto fileData = new UploadFileItemDto
                        {
                            ReMarks = (imagebytes.Length / 1024).ToString("0") + "Kb",
                            LinkKey = DeviceCodeSelected.DeviceId,
                            LinkTable = "devicecode",
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
        private async Task FileRead(string linkKey, string linkTable = "devicecode")
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
                        DeviceCodeImageInfo = item.Files[0];
                        DeviceCodeImage = CommAction.ByteArrayToBitmap(DeviceCodeImageInfo.FileBytes);
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
        /// 新建设备
        /// </summary>
        [RelayCommand]
        private async Task AddDeviceCode()
        {
            DeviceCodeSelected = new DeviceCodeItemDto();
            await Task.Delay(200);

            DeviceCodeSelected.IdxNum = 0;
            DeviceCodeSelected.ServiceLife = 10;
            DeviceCodeSelected.LifeUnit = "年";
            SetItemSelected();
            ImageClear();
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private void DeviceCodeSave()
        {
            try
            {
                StringBuilder _error = new StringBuilder();
                DeviceCodeSelected.DeviceType = DeviceTypeListSelected?.Id;
                if (DeviceStructList.Count == 0)
                {
                    _error.AppendLine($"请完善设备构成清单");
                }
                if (string.IsNullOrEmpty(DeviceCodeSelected.DeviceName))
                {
                    _error.AppendLine($"设备名称不能为空");
                }
                if (string.IsNullOrEmpty(DeviceCodeSelected.DeviceType))
                {
                    _error.AppendLine($"设备类型不能为空");
                }
                if (string.IsNullOrEmpty(DeviceCodeSelected.DeviceModel))
                {
                    _error.AppendLine($"设备型号不能为空");
                }
                if (string.IsNullOrEmpty(DeviceCodeSelected.Usage))
                {
                    _error.AppendLine($"设备用途不能为空");
                }
                if (DeviceCodeSelected.ServiceLife<1)
                {
                    _error.AppendLine($"设备使用年限不能为空");
                }
                if (string.IsNullOrEmpty(DeviceCodeSelected.LifeUnit))
                {
                    _error.AppendLine($"设备使用年限单位不能为空");
                }
                if (!string.IsNullOrEmpty(_error.ToString()))
                {
                    dialog.Error("提示", _error.ToString());
                    return;
                }

                dialog.ShowLoading("数据提交中...",async e => {
                    try
                    {
                        DeviceCodePostRequestDto request = new DeviceCodePostRequestDto
                        {
                            DeviceCodeItem = DeviceCodeSelected,
                            DeviceStructItems =[.. DeviceStructList]
                        };
                        var header = CommAction.GetHeader();
                        var data = await _client.DeviceCodePostAsync(_mapper.Map<DeviceCodePostRequest>(request), header);
                        
                        if (data.Code)
                        {
                            dialog.Success("提示", data.Message);

                            DeviceCodeSelected = _mapper.Map<DeviceCodeItemDto>(data.Item);
                            DeviceStructList =new ObservableCollection<DeviceStructItemDto>(_mapper.Map<List<DeviceStructItemDto>>(data.List));
                            ImageShow = true;
                            DeviceTreeLoad();
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
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 设备构成面板开关
        /// </summary>
        [ObservableProperty] private bool _structPanelOpen = false;
        /// <summary>
        /// 添加设备构成
        /// </summary>
        [RelayCommand]
        private void AddDeviceSturct()
        {
            StrucObjParse(new DeviceStructItemDto { 
                Yhid=DeviceCodeSelected.Yhid,
                StructId="",
                DeviceId=DeviceCodeSelected.DeviceId,
                IdxNum= DeviceStructList.Count + 1,
                MaterialId="",
                QuantityUnit = "个",
                UtilizeQuantity = 1
            });
            StructPanelOpen = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private void StrucObjParse(DeviceStructItemDto item)
        {
            TempDeviceStructObj = new DeviceStructGridItemDto
            {
                StructId = item.StructId,
                DeviceId = item.DeviceId,
                IdxNum = item.IdxNum,
                Yhid = item.Yhid,
                UtilizeQuantity = item.UtilizeQuantity,
                Material = CommAction.SetSelectedItem(MaterialCodes, item.MaterialId),
                Quantity = CommAction.SetSelectedItem(LifeUnitList, item.QuantityUnit)
            };
        }
        /// <summary>
        /// 设备构成编辑填充
        /// </summary>
        [RelayCommand]
        private void DeviceStructAdd()
        {
            if (TempDeviceStructObj != null)
            {
                DeviceStructItemDto item = new DeviceStructItemDto { 
                    Yhid=TempDeviceStructObj.Yhid,
                    StructId = TempDeviceStructObj.StructId,
                    DeviceId = TempDeviceStructObj.DeviceId,
                    IdxNum = TempDeviceStructObj.IdxNum,
                    MaterialId = TempDeviceStructObj.Material.Id,
                    MaterialName = TempDeviceStructObj.Material.Name,
                    QuantityUnit = TempDeviceStructObj.Quantity.Name,
                    UtilizeQuantity = TempDeviceStructObj.UtilizeQuantity
                };
                if (string.IsNullOrEmpty(item.MaterialId))
                {
                    dialog.Tips("提示", "请选择材料");
                    return;
                }
                if (string.IsNullOrEmpty(item.QuantityUnit))
                {
                    dialog.Tips("提示", "请选择材料计量单位");
                    return;
                }

                var existingItem = DeviceStructList.FirstOrDefault(x => x.StructId == item.StructId&&!string.IsNullOrEmpty(x.StructId));
                if (existingItem == null)
                {
                    DeviceStructList.Add(item);
                }
                else
                {
                    DeviceStructList.ReplaceOrAdd(existingItem,item);
                }
                TempDeviceStructObj.IdxNum = item.IdxNum + 1;
                TempDeviceStructObj.Material = new ComboboxItem("", "", ""); // 清空物资选择
            }
        }
        /// <summary>
        /// 关闭抽屉
        /// </summary>
        [RelayCommand]
        private void DeviceStructClose()
        {
            StructPanelOpen = false;
            TempDeviceStructObj = new DeviceStructGridItemDto();
        }
        /// <summary>
        /// 设备状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        public async Task DeviceState(string type,DeviceCodeTreeNodeDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if (type == "IsLock")
                {
                    DeviceCodeStatePost(type,node.Id,"");
                }
                else
                {
                    if (!node.IsLock)
                    {
                        var data = await dialog.Prompt("请输入锁定原因", "确定");
                        if (data.Item1)
                        {
                            DeviceCodeStatePost(type, node.Id, data.Item2);
                        }
                    }
                    else
                    {
                        DeviceCodeStatePost(type, node.Id, "");
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 设备状态变更请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Id"></param>
        /// <param name="Reason"></param>
        private void DeviceCodeStatePost(string type,string Id,string Reason)
        {
            dialog.ShowLoading("数据处理中...", async e => {
                DeviceCodeStateRequestDto request = new DeviceCodeStateRequestDto
                {
                    State = "IsDelete",
                    DeviceId = Id,
                    Reason = ""
                };
                var data = await _client.DeviceCodeStateAsync(_mapper.Map<DeviceCodeStateRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    dialog.Success("提示", data.Message);
                    //刷新树
                    DeviceTreeLoad();
                    if (type == "IsDelete")
                    {
                        _selectedTreeNode = new DeviceCodeTreeNodeDto();
                        DeviceCodeSelected = new DeviceCodeItemDto();
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
        /// <summary>
        /// 设备构成状态变更
        /// </summary>

        public void DeviceStuctState(string type, DeviceStructItemDto item)
        {
            try
            {
                if (type == "IsDelete")
                {
                    DeviceStructList.Remove(item);
                    if (!string.IsNullOrEmpty(item.StructId))
                    {
                        dialog.ShowLoading("构成材料删除中...",async e => {
                            DeviceStructStateRquestDto request = new DeviceStructStateRquestDto
                            {
                                State = "IsDelete",
                                StructId = item.StructId,
                                Reason = ""
                            };
                            var data = await _client.DeviceStructCodeStateAsync(_mapper.Map<DeviceStructStateRequest>(request), CommAction.GetHeader());
                            if (data.Code)
                            {
                                dialog.Success("删除提示",data.Message);
                            }
                            else
                            {
                                dialog.Error("提示",data.Message);
                            }
                        });
                        
                    }
                }
                else if (type == "IsEdit")
                {
                    TempDeviceStructObj = new DeviceStructGridItemDto
                    {
                        StructId = item.StructId,
                        DeviceId = item.DeviceId,
                        IdxNum = item.IdxNum,
                        Yhid = item.Yhid,
                        UtilizeQuantity = item.UtilizeQuantity,
                        Material = CommAction.SetSelectedItem(MaterialCodes, item.MaterialId),
                        Quantity = CommAction.SetSelectedItem(LifeUnitList, item.QuantityUnit)
                    };
                    StructPanelOpen = true;
                }
            }
            catch(Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
    }
    /// <summary>
    /// 设备构成对象
    /// </summary>
    public class DeviceStructGridItemDto : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public int IdxNum { get; set; } = 0;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 构成ID
        /// </summary>
        public string StructId { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// 物资
        /// </summary>
        public ComboboxItem Material { get; set; } = new ComboboxItem("", "", "");

        /// <summary>
        /// 使用数量
        /// </summary>
        public int UtilizeQuantity { get; set; } = 0;

        /// <summary>
        /// 计量单位
        /// </summary>
        public ComboboxItem Quantity { get; set; } = new ComboboxItem("", "", "");

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
