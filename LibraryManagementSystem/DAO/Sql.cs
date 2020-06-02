using FreeSql;
using LibraryManagementSystem.Config;
using LibraryManagementSystem.LogSys;
using System;

namespace LibraryManagementSystem.DAO
{
    public static class Sql
    {
        public static readonly string LogName = "Sql";
        // 单例模式
        private static IFreeSql sql;
        // 线程锁
        private static readonly object initLocker = new object();
        /// <summary>
        /// 唯一的数据库连接对象
        /// </summary>
        public static IFreeSql Instance
        {
            get
            {
                if (sql == null)
                {
                    lock (initLocker)
                    {
                        if (sql == null)
                        {
                            try
                            {
                                sql = new FreeSqlBuilder()
                                .UseConnectionString(Configuration.Instance.SqlDataType, Configuration.Instance.SqlConnectionString)
                                .UseAutoSyncStructure(true) //自动同步实体结构到数据库,生产环境中删除
                                .Build();
                            }
                            catch (Exception ex)
                            {
                                LoggerHolder.Instance.Error(ex, "{LogName}: 创建数据库对象失败",
                                    LogName);
                            }
                        }
                    }
                }
                return sql;
            }
        }
    }
}
