using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.Hosting;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace bbnApp.deskTop.Common
{
    public class Dialog: IDialog
    {
        private readonly ISukiDialogManager DialogManager;
        private readonly ISukiToastManager ToastManager;
        public Dialog(ISukiDialogManager dialogManager, ISukiToastManager toastManager) {
            DialogManager = dialogManager;
            ToastManager = toastManager;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DialogManager"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="okText"></param>
        /// <param name="cancelText"></param>
        /// <returns></returns>
        public async Task<bool> Confirm(string title = "提示", string content = "确定要进行操作吗？", string okText = "确定", string cancelText = "取消")
        {
            bool b = false;
            var tsc = new TaskCompletionSource<bool>();
            DialogManager.CreateDialog()
                .WithTitle(title)
                .WithContent(content)
                .WithActionButton(okText, _ =>
                {
                    b = true;
                    tsc.SetResult(true);
                }, true, "Flat")
                .WithActionButton(cancelText, _ =>
                {
                    tsc.SetResult(true);
                }, true, "Accent")  // 点击后关闭对话框
                .TryShow();
            await tsc.Task;
            return b;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DialogManager"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="okText"></param>
        /// <returns></returns>
        public async Task<bool> Alert(string title = "提示", string content = "确定要进行操作吗？", string okText = "确定")
        {
            bool b = false;
            var tsc = new TaskCompletionSource<bool>();
            DialogManager.CreateDialog()
                .WithTitle(title)
                .WithContent(content)
                .WithActionButton(okText, _ =>
                {
                    b = true;
                    tsc.SetResult(true);
                }, true, "Flat")
                .TryShow();
            await tsc.Task;
            return b;
        }
        /// <summary>
        /// 提示
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public void Tips(string title = "提示", string content = "操作提示", int delay = 3,Avalonia.Controls.Notifications.NotificationType type= Avalonia.Controls.Notifications.NotificationType.Information)
        {
            ToastManager.CreateToast()
                .WithTitle(title)
                .WithContent(content)
                .OfType(type)
                .Dismiss().ByClicking()
                .Dismiss().After(TimeSpan.FromSeconds(delay))
                .Queue();
        }
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="delay"></param>
        /// <param name="type"></param>
        public void Error(string title = "提示", string content = "操作提示", int delay = 3)
        {
            ToastManager.CreateToast()
                .WithTitle(title)
                .WithContent(content)
                .OfType(Avalonia.Controls.Notifications.NotificationType.Error)
                .Dismiss().ByClicking()
                .Dismiss().After(TimeSpan.FromSeconds(delay))
                .Queue();
        }

        /// <summary>
        /// 成功提示
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="delay"></param>
        public void Success(string title = "提示", string content = "操作成功", int delay = 3)
        {
            Dispatcher.UIThread.Invoke(() => { 
                ToastManager.CreateToast()
                .WithTitle(title)
                .WithContent(content)
                .OfType(Avalonia.Controls.Notifications.NotificationType.Success)
                .Dismiss().ByClicking()
                .Dismiss().After(TimeSpan.FromSeconds(delay))
                .Queue();
            });
        }
        /// <summary>
        /// 进度框
        /// </summary>
        /// <param name="sukiToastManager"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public (ISukiToast,Timer) Loading(string title = "数据处理中")
        {
            var progress = new ProgressBar() { Value = 0, ShowProgressText = true };
            var toast = ToastManager.CreateToast()
                .WithTitle(title)
                .WithContent(progress)
                .Queue();

            var timer = new Timer(500);
            timer.Elapsed += (_, _) =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    progress.Value += 5;
                    if (progress.Value < 100) return;
                    timer.Dispose();
                    ToastManager.Dismiss(toast);

                });
            };
            timer.Start();
            return (toast, timer);
        }
        /// <summary>
        /// 进度-封装
        /// </summary>
        /// <param name="title"></param>
        /// <param name="ac"></param>
        public void ShowLoading(string title="请求处理中",Action<(ISukiToast, Timer)> ac=null)
        {
            var (toast, timer) = Loading(title);
            try
            {
                if (ac != null)
                {
                    ac((toast, timer));
                }
            }
            catch (Exception ex)
            {
                ToastManager.CreateToast()
                    .WithTitle("异常提示")
                    .WithContent($"请求产生异常：{ex.Message.ToString()}")
                    .OfType(Avalonia.Controls.Notifications.NotificationType.Error)
                    .Dismiss().ByClicking()
                    .Dismiss().After(TimeSpan.FromSeconds(3))
                    .Queue();

                LoadingClose((toast,timer));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void LoadingClose((ISukiToast,Timer) e)
        {
            if (e.Item1 != null)
            {
                ToastManager.Dismiss(e.Item1);
            }
            if (e.Item2 != null)
            {
                e.Item2.Dispose();
            }
        }
    }
}
