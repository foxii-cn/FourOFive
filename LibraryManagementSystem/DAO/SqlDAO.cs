using FreeSql;
using LibraryManagementSystem.Models;
using Serilog.Core;
using System;

namespace LibraryManagementSystem.DAO
{
    public class SqlDAO
    {
        // 配置对象
        private readonly Config config;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        // FreeSql数据库连接字符串Raw
        private static readonly string MySqlConnStr = @"Data Source={0};Port={1};User ID={2};Password={3}; Initial Catalog={4};Charset={5}; SslMode={6};Min pool size={7}";
        private static readonly string PostgreSQLConnStr = @"Host={0};Port={1};Username={2};Password={3}; config.Database={4};Pooling={5};Minimum Pool Size={6}";
        private static readonly string SqlServerConnStr = @"Data Source={0};Integrated Security={1};Initial Catalog={2};Pooling={3};Min Pool Size={4}";
        private static readonly string OracleConnStr = @"user id={0};password={1}; data source={2};Pooling={3};Min Pool Size={4}";
        private static readonly string SqliteConnStr = @"Data Source={0}; Attachs={1}; Pooling={2};Min Pool Size={3}";

        /// <summary>
        /// FreeSql数据库类型
        /// </summary>
        private DataType SqlDataType
        {
            get
            {
                return (config.DatabaseType.ToLower()) switch
                {
                    "mysql" => DataType.MySql,
                    "postgresql" => DataType.PostgreSQL,
                    "sqlserver" => DataType.SqlServer,
                    "oracle" => DataType.Oracle,
                    "sqlite" => DataType.Sqlite,
                    _ => DataType.Sqlite,
                };
            }
        }
        /// <summary>
        /// FreeSql数据库连接字符串
        /// </summary>
        private string SqlConnectionString
        {
            get
            {
                return (config.DatabaseType.ToLower()) switch
                {
                    "mysql" => string.Format(MySqlConnStr, config.DatabaseDataSource, config.DatabasePort, config.DatabaseUser, config.DatabasePassword, config.Database, config.DatabaseCharset, config.DatabaseSecurity, config.DatabaseMinPoolSize),
                    "postgresql" => string.Format(PostgreSQLConnStr, config.DatabaseDataSource, config.DatabasePort, config.DatabaseUser, config.DatabasePassword, config.Database, config.DatabasePooling, config.DatabaseMinPoolSize),
                    "sqlserver" => string.Format(SqlServerConnStr, config.DatabaseDataSource, config.DatabaseSecurity, config.Database, config.DatabasePooling, config.DatabaseMinPoolSize),
                    "oracle" => string.Format(OracleConnStr, config.DatabaseUser, config.DatabasePassword, config.DatabaseDataSource, config.DatabasePooling, config.DatabaseMinPoolSize),
                    "sqlite" => string.Format(SqliteConnStr, config.DatabaseDataSource, config.DatabaseAttachs, config.DatabasePooling, config.DatabaseMinPoolSize),
                    _ => string.Format(SqliteConnStr, config.DatabaseDataSource, config.DatabaseAttachs, config.DatabasePooling, config.DatabaseMinPoolSize),
                };
            }
        }


        public SqlDAO(Config config, Logger logger)
        {
            LogName = GetType().Name;
            this.config = config;
            this.logger = logger;
        }
        public IFreeSql GetSql()
        {
            IFreeSql sql = null;
            try
            {
                sql = new FreeSqlBuilder()
                .UseConnectionString(SqlDataType, SqlConnectionString)
                .UseAutoSyncStructure(config.DatabaseAutoSyncStructure)
                .Build();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 创建数据库对象失败",
                    LogName);
            }
            return sql;
        }


    }
}
