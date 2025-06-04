using Avalonia.Controls.Notifications;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Avalonia.Collections;
using bbnApp.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using bbnApp.Share;
using bbnApp.DTOs.CodeDto;
using AutoMapper;
using bbnApp.deskTop.Common;

namespace bbnApp.deskTop.ViewModels
{
    public partial class LoginWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// 登录状态
        /// </summary>
        [ObservableProperty] private bool _isLoggingIn = false;
        /// <summary>
        /// 机构请求状态
        /// </summary>
        [ObservableProperty] private bool _isCompanyReload = false;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private string _username;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private string _password;
        /// <summary>
        /// 机构清单
        /// </summary>
        [ObservableProperty] private AvaloniaList<CompanyItemDto> _companyItems;
        /// <summary>
        /// 选中的机构
        /// </summary>
        [ObservableProperty] private CompanyItemDto _selectedCompanyItem;
        /// <summary>
        /// 
        /// </summary>
        private readonly IRedisService _redisService;

        private readonly IDialog dialog;

        private readonly Author.AuthorClient client;
        private readonly CompanyGrpcService.CompanyGrpcServiceClient companyclient;

        private readonly ISukiDialog window;

        private readonly Action<ISukiDialog, bool, string, LoginResponse> ac;

        private readonly IMapper mapper;

        public LoginWindowViewModel(ISukiDialog window, Author.AuthorClient client, CompanyGrpcService.CompanyGrpcServiceClient companyclient, IDialog dialog, Action<ISukiDialog, bool, string, LoginResponse> ac, List<CompanyItemDto> companyItems, IRedisService _redisService,IMapper _mapper)
        {
            this._redisService = _redisService;
            this.window = window;
            this.client = client;
            this.companyclient = companyclient;
            this.dialog = dialog;
            this.ac = ac;
            CompanyItems = companyItems==null?[]:[.. companyItems];//机构信息
            mapper = _mapper;
            _ =InitializeAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            string LogUser =await _redisService.GetAsync("bbnLoginUser");
            if (!string.IsNullOrEmpty(LogUser))
            {
                JObject ObjUser = JObject.Parse(LogUser);
                if (ObjUser != null)
                {
                    Username = ObjUser["UserName"]?.ToObject<string>();
                    string? yhid = ObjUser["Yhid"]?.ToObject<string>();
                    string? companyid = ObjUser["CompanyId"]?.ToObject<string>();
                    //匹配机构
                    CompanyItemDto item=CompanyItems.Where(x=>x.Yhid==yhid &&x.Id==companyid).FirstOrDefault();
                    if (item != null)
                    {
                        SelectedCompanyItem = item;
                    }
                }
            }
        }
        /// <summary>
        /// 机构重载
        /// </summary>
        [RelayCommand]
        private async Task CompanyReload()
        {
            try
            {
                IsCompanyReload = true;
                var (b,msg,_items) = await BasicRequest.CompanyItemsLoad(companyclient, mapper);
                if (!b)
                {
                    dialog.Error("提示", msg);
                }
                CompanyItems = [.._items];
                if (b) {
                    dialog.Success("提示", "机构请求完成", 2);
                }
                else
                {
                    dialog.Error("提示", "机构请求失败", 2);
                }
                    
                await InitializeAsync();
            }
            catch(Exception ex)
            {
                dialog.Error("提示", "机构加载失败");
            }
            finally
            {
                IsCompanyReload = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [RelayCommand]
        private void CompanySelected(object sender)
        {
            var c = sender;
        }
        /// <summary>
        /// 密码输入回车提交
        /// </summary>
        /// <param name="sender"></param>
        
        public async Task PassWordKeyPress()
        {
            await Login();
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task Login()
        {
            try
            {
                if (SelectedCompanyItem == null)
                {
                    dialog.Error("提示", "请选择所在的机构");
                    return;
                }
                else if (string.IsNullOrEmpty(Username))
                {
                    dialog.Error("提示", "登录账号/工号不能为空");
                    return;
                }
                else if (string.IsNullOrEmpty(Password))
                {
                    dialog.Error("提示", "登录密码不能为空");
                    return;
                }
                if (!IsLoggingIn)
                {
                    IsLoggingIn = true;
                    LoginRequest _loginRequest = new LoginRequest
                    {
                        Yhid = SelectedCompanyItem.Yhid,
                        CompanyId = SelectedCompanyItem.Id,
                        UserName = Username,
                        PassWord = Password,
                        LoginFrom = "DeskTop",
                        IPAddress =CommMethod.GetLocalIPAddress(),
                        AreaInfo = "/"
                    };
                    var response = await client.LoginAsync(_loginRequest);
                    IsLoggingIn = false;
                    if (response.Code)
                    {
                        //本次登录信息存储在本地(加密保存)
                        await _redisService.SetAsync("bbnLoginUser", JsonConvert.SerializeObject(new JObject { { "Yhid", _loginRequest.Yhid }, { "CompanyId", _loginRequest.CompanyId }, { "UserName", _loginRequest.UserName } }));

                        ac(window, true, response.Message, response);
                    }
                    else
                    {
                        Username = string.Empty;
                        Password = string.Empty;
                        dialog.Error("登录失败", response.Message);
                    }
                }
                else
                {
                    dialog.Tips("进度提示", "正在处理中,请勿重复操作",2,NotificationType.Warning);
                }
            }
            catch(RpcException ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
            finally
            {
                IsLoggingIn = false;
            }
        }
        [RelayCommand]
        private void CloseDialog() {
            ac(window,false, string.Empty,null);
        }
    }
}
