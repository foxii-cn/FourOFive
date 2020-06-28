using System;
using System.Data.Common;

namespace FourOFive.Managers
{
    /// <summary>
    /// 事务管理类, 方便扩展系统自带的DbTransaction类
    /// </summary>
    public interface IDatabaseTransactionManager : IDisposable
    {
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit();
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback();
        /// <summary>
        /// 获取事务的DbTransaction对象
        /// </summary>
        /// <returns></returns>
        public DbTransaction GetTransaction();
    }
}
