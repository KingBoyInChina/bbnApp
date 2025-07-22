using AutoMapper;
using Avalonia.Controls;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;

using bbnApp.deskTop.Services;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.MQTT.Client;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using MQTTnet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bbnApp.deskTop.OperationsCenter.MQTTClients
{
    /// <summary>
    /// MQTT实时连接情况
    /// </summary>
    partial class MQTTClientsViewModel:BbnPageBase
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
        /// 密钥
        /// </summary>
        private ReigisterKeyGrpcService.ReigisterKeyGrpcServiceClient _client;
        /// <summary>
        /// 
        /// </summary>
        private List<AuthorReginsterKeyClientDto> RegisterKeys = new List<AuthorReginsterKeyClientDto>();
        /// <summary>
        /// 当前连接对象
        /// </summary>
        [ObservableProperty] private ObservableCollection<MqttClientModel> _connectionClients = new ObservableCollection<MqttClientModel>();
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        private readonly MqttClientService _mqttClientService;
        /// <summary>
        /// Cient分类,Operator/User/Device(操作员/用户/设备)
        /// </summary>
        [ObservableProperty] private string _clientType = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public MQTTClientsViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog, MqttClientService _mqttClientService) : base("OperationsCenter", "MQTT连接管理", MaterialIconKind.Connection, "", 6)
        {
            _ = ClientInit(grpcClientFactory);
            this.dialogManager = DialogManager;

            this.nav = nav;
            this.dialog = dialog;
            _mapper = mapper;
            this._mqttClientService = _mqttClientService;
        }

        private async Task ClientInit(IGrpcClientFactory grpcClientFactory)
        {
            _client = await grpcClientFactory.CreateClient<ReigisterKeyGrpcService.ReigisterKeyGrpcServiceClient>();
            await OnActivated();
        }

        #region 实时连接对象
        /// <summary>
        /// 初始化中注册的密钥信息
        /// </summary>
        public async Task GetRegisterKeys(UserControl u)
        {
            try
            {
                NowControl = u;
                var request = new AuthorRegisterKeySearchRequestDto
                {
                    SetAppCode="",
                    SetAppName="",
                    CompanyId="",
                    AppId=""
                };
                var data = await _client.AuthorRegisterKeySearchAsync(_mapper.Map<AuthorRegisterKeySearchRequest>(request),CommAction.GetHeader());
                if (data.Code)
                {
                    RegisterKeys =_mapper.Map<List<AuthorReginsterKeyClientDto>>( data.Items);
                }
                else
                {
                    dialog.Error("提示", data.Message);
                }
            }
            catch(Exception ex)
            {
                dialog.Error("提示",$"初始化注册密钥信息异常：{ex.Message.ToString()}");
            }
        }
        #endregion
        #region 动态添加订阅
        /// <summary>
        /// 实时连接客户端-订阅主题
        /// </summary>
        private string topic = $"/Private/Operator/{UserContext.CurrentUser.OperatorId}/GetClients";
        /// <summary>
        /// 动态订阅主题
        /// </summary>
        public async Task OnActivated()
        {
            await _mqttClientService.Subscribe(topic);
            _mqttClientService.RegisterHandler(topic, OnDataReceived);
        }
        /// <summary>
        /// 取消订阅并移除处理逻辑
        /// </summary>
        public void OnDeactivated()
        {
            _mqttClientService.UnregisterHandler(topic, OnDataReceived);
        }
        /// <summary>
        /// 接收到消息处理
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="payload"></param>
        private void OnDataReceived(string topic, string payload)
        {
            // 处理收到的数据
            var list = RegisterKeys.Where(x=> payload.Contains(x.AuthorId));
            ConnectionClients.Clear();
            #region 数据过滤
            if (!string.IsNullOrEmpty(ClientType))
            {

            }
            #endregion

            //ConnectionClients = [..list];
            IsBusy = false;
        }
        #endregion
        /// <summary>
        /// 实时连接对象重载
        /// </summary>
        [RelayCommand]
        private async Task ClientsLoad()
        {
            await MQTTClientsLoad();
        }
        /// <summary>
        /// 
        /// </summary>
        private async Task MQTTClientsLoad()
        {
            try
            {
                IsBusy = true;
                await _mqttClientService.PublishAsync("/MqttService/GetClients", "获取实时连接对象");
            }
            catch(Exception ex)
            {
                IsBusy = false;
                dialog.Error("提示",$"实时连接对象获取异常：{ex.Message.ToString()}");
            }
        }
    }
}
