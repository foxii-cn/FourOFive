using System;

namespace FourOFive.Managers
{
    /// <summary>
    /// 日志记录器工厂
    /// </summary>
    public interface ILogManagerFactory : IDisposable
    {
        /// <summary>
        /// 创建一个日志记录器
        /// </summary>
        /// <typeparam name="T">使用日志记录器的类, 将会使用类名作为日志的标识</typeparam>
        /// <returns>带T类名的日志记录器</returns>
        public ILogManager CreateManager<T>() where T : class;
    }
}
