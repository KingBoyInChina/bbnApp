using Avalonia.Controls.Notifications;
using SukiUI.Toasts;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace bbnApp.deskTop.Common
{
    public interface IDialog
    {
        /// <summary>
        /// Confirm
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="okText"></param>
        /// <param name="cancelText"></param>
        /// <returns></returns>
        Task<bool> Confirm(string title = "提示", string content = "确定要进行操作吗？", string okText = "确定", string cancelText = "取消");
        /// <summary>
        /// Alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="okText"></param>
        /// <returns></returns>
        Task<bool> Alert(string title = "提示", string content = "确定要进行操作吗？", string okText = "确定");
        /// <summary>
        /// 错误提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="delay"></param>
        void Error(string title = "提示", string content = "操作提示", int delay = 3);
        /// <summary>
        /// 提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="delay"></param>
        void Tips(string title = "提示", string content = "操作提示", int delay = 3, Avalonia.Controls.Notifications.NotificationType type = Avalonia.Controls.Notifications.NotificationType.Information);
        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="delay"></param>
        void Success(string title = "提示", string content = "操作成功", int delay = 3);
        /// <summary>
        /// loading
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        (ISukiToast, Timer) Loading(string title = "数据处理中");
        /// <summary>
        /// 进度
        /// </summary>
        /// <param name="title"></param>
        /// <param name="ac"></param>
        void ShowLoading(string title = "请求处理中", Action<(ISukiToast, Timer)> ac = null);
        /// <summary>
        /// 进度关闭
        /// </summary>
        /// <param name="e"></param>
        void LoadingClose((ISukiToast, Timer) e);
    }
}
