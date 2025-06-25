using AutoMapper;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.ViewModels;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace bbnApp.deskTop.OrganizationStructure.ReigisterKey
{
    partial class RegisterKeyEditViewModel : ViewModelBase
    {
        /// <summary>
        /// 映射
        /// </summary>
        private IMapper _mapper;
        /// <summary>
        /// 提交回调
        /// </summary>
        private Action<bool, string, object> _registerKeySubmitCallBack;
        /// <summary>
        /// 
        /// </summary>
        private IDialog dialog;
        /// <summary>
        /// 状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 
        /// </summary>
        private ReigisterKeyGrpcService.ReigisterKeyGrpcServiceClient _client;

        private AuthorRegisterKeyItemDto _tempData;

        /// <summary>
        /// 密钥对象
        /// </summary>
        [ObservableProperty] private AuthorRegisterKeyItemDto _registerKeyItem = new AuthorRegisterKeyItemDto();

        public RegisterKeyEditViewModel()
        {
            
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="Node"></param>

        public void ViewModelInit(IDialog dialog,Action<bool, string, object> CallBack, AuthorRegisterKeyItemDto InitValue, ReigisterKeyGrpcService.ReigisterKeyGrpcServiceClient client, IMapper _mapper)
        {
            this.dialog = dialog;
            _registerKeySubmitCallBack = CallBack;
            _tempData = InitValue;
            _client = client;
            this._mapper = _mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ViewModelInit()
        {
            RegisterKeyItem = _tempData;
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private async Task RegisterSubmit()
        {
            try
            {
                string _msg = string.Empty;
                if (string.IsNullOrEmpty(RegisterKeyItem.SetAppName))
                {
                    _msg = "注册应用名称不能为空";
                }
                else if (string.IsNullOrEmpty(RegisterKeyItem.SetAppDescription))
                {
                    _msg = "应用用途说明不能为空";
                }
                else
                {
                    IsBusy = true;
                    AuthorRegisterKeyAddRequestDto request = new AuthorRegisterKeyAddRequestDto
                    {
                        Item = RegisterKeyItem,
                    };

                    var header = CommAction.GetHeader();

                    var response = await _client.AuthorRegisterKeyAddAsync(_mapper.Map<AuthorRegisterKeyAddRequest>(request), header);
                    if (response.Code)
                    {
                        _registerKeySubmitCallBack(response.Code, response.Message, response.Item);
                    }
                    else
                    {
                        dialog.Error("错误提示", $"数据提交错误：{response.Message}");
                    }
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                dialog.Error("异常提示", $"数据提交异常：{ex.Message.ToString()}");
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
            bool b = await dialog.Confirm("关闭提示", "确定要关闭当前密钥申请页面吗？", "关闭", "取消");
            if (b)
            {
                _registerKeySubmitCallBack(false, "关闭", null);
            }
        }
    }
}
