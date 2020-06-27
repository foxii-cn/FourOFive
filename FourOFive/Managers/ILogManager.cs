using System;

namespace FourOFive.Managers
{
    public interface ILogManager
    {
        public void Verbose(string messageTemplate, params object[] propertyValues);
        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues);
        public void Debug(string messageTemplate, params object[] propertyValues);
        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues);
        public void Info(string messageTemplate, params object[] propertyValues);
        public void Info(Exception exception, string messageTemplate, params object[] propertyValues);
        public void Warn(string messageTemplate, params object[] propertyValues);
        public void Warn(Exception exception, string messageTemplate, params object[] propertyValues);
        public void Error(string messageTemplate, params object[] propertyValues);
        public void Error(Exception exception, string messageTemplate, params object[] propertyValues);
        public void Fatal(string messageTemplate, params object[] propertyValues);
        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);

    }
}
