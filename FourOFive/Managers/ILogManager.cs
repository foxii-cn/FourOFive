using System;

namespace FourOFive.Managers
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// 记录Verbose级别的日志
        /// </summary>
        /// <param name="messageTemplate">消息模板, 行为类似WriteLine</param>
        /// <param name="propertyValues">要填充的对象</param>
        public void Verbose(string messageTemplate, params object[] propertyValues);
        /// <summary>
        /// 记录Verbose级别的日志
        /// </summary>
        /// <param name="exception">要打印的异常</param>
        /// <param name="messageTemplate">消息模板, 行为类似WriteLine</param>
        /// <param name="propertyValues">要填充的对象</param>
        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues);
        /// <summary>
        /// 记录Debug级别的日志
        /// </summary>
        /// <param name="messageTemplate">消息模板, 行为类似WriteLine</param>
        /// <param name="propertyValues">要填充的对象</param>
        public void Debug(string messageTemplate, params object[] propertyValues);
        /// <summary>
        /// 记录Debug级别的日志
        /// </summary>
        /// <param name="exception">要打印的异常</param>
        /// <param name="messageTemplate">消息模板, 行为类似WriteLine</param>
        /// <param name="propertyValues">要填充的对象</param>
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
