using Avalonia.Threading;
using Exceptionless;
using Serilog;
using System;
using System.Threading.Tasks;

namespace bbnApp.deskTop.Services
{
    public class ExceptionService
    {
        private readonly ILogger _logger;

        private readonly ExceptionlessClient _exceptionlessClient;

        public ExceptionService(ILogger logger, ExceptionlessClient exceptionlessClient)
        {
            _logger = logger;
            _exceptionlessClient = exceptionlessClient;
        }
        /// <summary>
        /// UI异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // 处理 UI 线程中的未处理异常
            HandleException(e.Exception, "UI 线程");

            // 标记为已处理，避免应用崩溃
            e.Handled = true;
        }
        /// <summary>
        /// 非UI异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 处理非 UI 线程中的未处理异常
            if (e.ExceptionObject is Exception ex)
            {
                HandleException(ex, "非 UI 线程");
            }
        }
        /// <summary>
        /// 异步任务异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // 处理异步任务中的未处理异常
            HandleException(e.Exception, "异步任务");

            // 标记为已观察，避免应用崩溃
            e.SetObserved();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="source"></param>
        public void HandleException(Exception ex, string source)
        {

            // 记录日志
            string logMessage = $"[{DateTime.Now}] 来源: {source}, 异常: {ex}\n";

            //异常监控
            _exceptionlessClient.CreateException(ex)
                    .SetProperty("Source", source)
                    .Submit();
            _logger.Error(logMessage);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exceptionlessClient"></param>
        /// <param name="ex"></param>
        /// <param name="source"></param>
        public static void HandleException(ILogger logger, ExceptionlessClient exceptionlessClient,Exception ex, string source)
        {

            // 记录日志
            string logMessage = $"[{DateTime.Now}] 来源: {source}, 异常: {ex}\n";

            //异常监控
            exceptionlessClient.CreateException(ex)
                    .SetProperty("Source", source)
                    .Submit();
            logger.Error(logMessage);
        }
    }
}
