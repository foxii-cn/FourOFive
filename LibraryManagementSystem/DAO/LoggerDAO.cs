using Serilog;
using Serilog.Core;

namespace LibraryManagementSystem.DAO
{
    public class LoggerDAO
    {
        private readonly string logFilePath;// = @".\logs\log.txt";
        private readonly long logFileSizeLimitBytes;// = 268435456;


        public LoggerDAO(string logFilePath, long logFileSizeLimitBytes)
        {
            this.logFilePath = logFilePath;
            this.logFileSizeLimitBytes = logFileSizeLimitBytes;
        }
        public Logger GetLogger()
        {
            return new LoggerConfiguration()
                                .MinimumLevel.Debug()
                                .WriteTo.Async(c => c.File(logFilePath, fileSizeLimitBytes: logFileSizeLimitBytes, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Day, shared: true))
                                .WriteTo.Async(c => c.Trace())
                                .CreateLogger();
        }
        public void Close(Logger logger)
        {
            logger.Dispose();
        }
    }
}
