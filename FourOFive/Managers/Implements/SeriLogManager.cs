using Serilog;
using System;

namespace FourOFive.Managers.Implements
{
    public class SeriLogManager : ILogManager
    {
        private readonly ILogger logger;
        public SeriLogManager(ILogger logger)
        {
            this.logger = logger;
        }
        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            logger.Debug(messageTemplate, propertyValues);
        }

        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            logger.Debug(exception, messageTemplate, propertyValues);
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            logger.Error(messageTemplate, propertyValues);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            logger.Error(exception, messageTemplate, propertyValues);
        }

        public void Fatal(string messageTemplate, params object[] propertyValues)
        {
            logger.Fatal(messageTemplate, propertyValues);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            logger.Fatal(exception, messageTemplate, propertyValues);
        }

        public void Info(string messageTemplate, params object[] propertyValues)
        {
            logger.Information(messageTemplate, propertyValues);
        }

        public void Info(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            logger.Information(exception, messageTemplate, propertyValues);
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            logger.Verbose(messageTemplate, propertyValues);
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            logger.Verbose(exception, messageTemplate, propertyValues);
        }

        public void Warn(string messageTemplate, params object[] propertyValues)
        {
            logger.Warning(messageTemplate, propertyValues);
        }

        public void Warn(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            logger.Warning(exception, messageTemplate, propertyValues);
        }
    }
}
