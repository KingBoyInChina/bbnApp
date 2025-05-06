using AutoMapper;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.ViewModels;
using bbnApp.DTOs.CodeDto;
using BbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace bbnApp.deskTop.PlatformManagement.DictionaryCode
{
    partial class DictionaryItemViewModel : ViewModelBase
    {
        private IMapper mapper;
        /// <summary>
        /// 
        /// </summary>
        private readonly IDialog dialog;

        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 选中的字典类目
        /// </summary>
        [ObservableProperty] private DataDictionaryCodeDto _selectedNode;
        /// <summary>
        /// 提交的字典项目
        /// </summary>
        [ObservableProperty] private DataDictionaryItemDto _selectedItem = new DataDictionaryItemDto();
        /// <summary>
        /// 
        /// </summary>
        private Action<bool, string, object> _submitCallBack;
        private DataDictionaryCodeDto _tempNode;
        private DataDictionaryItemDto _tempItem;
        private DataDictionaryGrpc.DataDictionaryGrpcClient _client;

        public DictionaryItemViewModel(IDialog dialog)
        {
            this.dialog = dialog;
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="InitValue"></param>
        /// <param name="client"></param>
        public void ViewModelInit(Action<bool, string, object> CallBack, DataDictionaryCodeDto Node, DataDictionaryItemDto Item, DataDictionaryGrpc.DataDictionaryGrpcClient client, IMapper mapper)
        {
            _submitCallBack = CallBack;
            _tempNode = Node;
            _client = client;
            _tempItem = Item;
            this.mapper = mapper;
        }
        /// <summary>
        /// 页面加载完成后初始化参数
        /// </summary>
        public void ViewModelInit()
        {
            SelectedNode = _tempNode;
            SelectedItem = _tempItem;
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private async Task DataSubmit()
        {
            IsBusy = true;
            try
            {
                string error = string.Empty;
                if (string.IsNullOrEmpty(SelectedNode.DicName))
                {
                    error = "字典名称不能为空";
                }
                if (!string.IsNullOrEmpty(error))
                {
                    dialog.Error("提示", error);
                }
                else
                {
                    DataDictionaryItemSaveRequestDto request = new DataDictionaryItemSaveRequestDto
                    {
                        Item = SelectedItem,
                    };
                    var data = await _client.ItemPostAsync(mapper.Map<DataDictionaryItemSaveRequest>(request), CommAction.GetHeader());
                    if (data.Code)
                    {
                        if (string.IsNullOrEmpty(SelectedItem.ItemId))
                        {
                            bool b = await dialog.Confirm("提示", $"是否需要继续添加{SelectedNode.DicName}的字典吗?", "是", "否");
                            if (b)
                            {
                                SelectedItem.ItemName = string.Empty;
                                await Task.Delay(500);
                                SelectedItem.ItemName = string.Empty;
                                SelectedItem.ItemId = SelectedItem.ItemId + 1;
                                SelectedItem.ReMarks = string.Empty;
                            }
                            else
                            {
                                _submitCallBack(true, "", data.Item);
                            }
                        }
                        else
                        {
                            _submitCallBack(true, data.Message, data.Item);
                        }
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }
                }

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
        /// 关闭
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task Close()
        {
            bool b = await dialog.Confirm("关闭提示", "确定要关闭当前字典项目维护页面吗？", "关闭", "取消");
            if (b)
            {
                _submitCallBack(false, "关闭", null);
            }
        }
    }
}
