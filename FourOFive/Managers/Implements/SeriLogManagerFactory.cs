using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace FourOFive.Managers.Implements
{
    public class SeriLogManagerFactory : ILogManagerFactory
    {
        private readonly Logger logger;
        private readonly IConfigurationManager config;
        private readonly string path = "./logs/log.txt";
        private readonly long size = 268435456;
        private readonly LogEventLevel level = LogEventLevel.Information;
        private readonly ILogManager selfLogger;

        public SeriLogManagerFactory(IConfigurationManagerFactory loggerFactory)
        {
            config = loggerFactory.CreateManager("Logger");
            path = config.Get("FilePath", path);
            size = config.Get("FileSizeLimitBytes", size);
            level = config.Get("MinimumLevel", level);
            logger = new LoggerConfiguration()
                .MinimumLevel.Is(level)
                .WriteTo.Async(c =>
                c.File(path,
                fileSizeLimitBytes: size,
                rollOnFileSizeLimit: true,
                rollingInterval: RollingInterval.Day,
                shared: true,
                outputTemplate: "{Timestamp:HH:mm:ss} [{SourceClass}] [{Level}] {Message:lj}{NewLine}{Exception}"))
                .WriteTo.Async(c =>
                c.Trace(outputTemplate: "{Timestamp:HH:mm:ss} [{SourceClass}] [{Level}] {Message:lj}{NewLine}{Exception}"))
                .CreateLogger();
            selfLogger = CreateManager<SeriLogManagerFactory>();
            selfLogger.Info("日志模块初始化完毕!");
        }

        public ILogManager CreateManager<T>() where T : class
        {
            return new SeriLogManager(logger.ForContext("SourceClass", typeof(T).Name));
        }

        public void Dispose()
        {
            selfLogger.Info("日志模块正在关闭...");
            logger.Dispose();
        }
    }
}
