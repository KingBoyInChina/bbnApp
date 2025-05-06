using AutoMapper;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.ViewModels;
using bbnApp.DTOs.CodeDto;
using BbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.PlatformManagement.DictionaryCode
{
    partial class DictionaryCodeEditViewModel: ViewModelBase
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
        /// 提交对象
        /// </summary>
        [ObservableProperty] private DataDictionaryCodeDto _selectedNode=new DataDictionaryCodeDto();
        /// <summary>
        /// 选中的树节点
        /// </summary>
        [ObservableProperty] private DicTreeItemDto _selectedTree;
        /// <summary>
        /// 
        /// </summary>
        private Action<bool, string, object> _submitCallBack;
        private DataDictionaryCodeDto _tempNode;
        private DicTreeItemDto _tempTree;
        private DataDictionaryGrpc.DataDictionaryGrpcClient _client;

        public DictionaryCodeEditViewModel(IDialog dialog) {
            this.dialog = dialog;
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="InitValue"></param>
        /// <param name="client"></param>
        public void ViewModelInit(Action<bool, string, object> CallBack, DataDictionaryCodeDto InitValue, DicTreeItemDto TreeNode, DataDictionaryGrpc.DataDictionaryGrpcClient client, IMapper mapper)
        {
            _submitCallBack = CallBack;
            _tempNode = InitValue;
            _client = client;
            _tempTree = TreeNode;
            this.mapper = mapper;
        }
        /// <summary>
        /// 页面加载完成后初始化参数
        /// </summary>
        public void ViewModelInit()
        {
            SelectedNode = _tempNode;
            SelectedTree= _tempTree;
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
                    error = "类目名称不能为空";
                }
                if (!string.IsNullOrEmpty(error))
                {
                    dialog.Error("提示",error);
                }
                else
                {
                    DataDictionarySaveRequestDto request = new DataDictionarySaveRequestDto
                    {
                        Item = SelectedNode,
                    };
                    var data = await _client.DicPostAsync(mapper.Map<DataDictionarySaveRequest>(request),CommAction.GetHeader());
                    if (data.Code)
                    {
                        dialog.Success("提示",data.Message);
                        if (string.IsNullOrEmpty(SelectedNode.DicCode))
                        {
                            bool b = await dialog.Confirm("提示",$"是否需要继续在{SelectedTree.Name}下添加类目?","是","否");
                            if (b)
                            {
                                SelectedNode.DicName = string.Empty;
                                await Task.Delay(500);
                                SelectedNode.DicCode = string.Empty;
                                SelectedNode.ReMarks = string.Empty;
                            }
                            else
                            {
                                _submitCallBack(true, "关闭", data.Item);
                            }
                        }
                        else
                        {
                            _submitCallBack(true, "关闭", data.Item);
                        }
                    }
                    else
                    {
                        dialog.Error("提示",data.Message);
                    }
                }

            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
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
            bool b = await dialog.Confirm("关闭提示", "确定要关闭当前字典类目维护页面吗？", "关闭", "取消");
            if (b)
            {
                _submitCallBack(false, "关闭", null);
            }
        }
    }
}
