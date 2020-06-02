using Serilog;
using Serilog.Core;

namespace LibraryManagementSystem.LogSys
{
    public static class LoggerHolder
    {
        public static readonly string logFilePath = @".\logs\log.txt";
        public static readonly long logFileSizeLimitBytes = 268435456;
        // 单例模式
        private static Logger logger;
        // 线程锁
        private static readonly object initLocker = new object();
        /// <summary>
        /// 日志记录器的唯一实例
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (logger == null)
                {
                    lock (initLocker)
                    {
                        if (logger == null)
                        {
                            logger = new LoggerConfiguration()
                                .MinimumLevel.Debug()
                                .WriteTo.Async(c => c.File(logFilePath, fileSizeLimitBytes: logFileSizeLimitBytes, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Day, shared: true))
                                .WriteTo.Async(c => c.Trace())
                                .CreateLogger();
                        }
                    }
                }
                return logger;
            }
        }
        public static void Close()
        {
            if (logger != null)
                logger.Dispose();
        }
    }
}
