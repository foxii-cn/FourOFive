using FreeSql;
using LibraryManagementSystem.LogSys;
using LibraryManagementSystem.Tool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Config
{
    public class Configuration
    {
        // 配置文件位置
        public static readonly string path = @".\config.json";
        // 文件编码
        public static readonly Encoding encoding = Encoding.UTF8;

        public static readonly string LogName = "Configuration";
        // 单例模式
        private static Configuration configuration;
        // 线程锁
        private static readonly object initLocker = new object();
        // 隐藏构造函数
        private Configuration() { }
        /// <summary>
        /// 配置类的唯一实例
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                // TODO
                if (configuration == null)
                {
                    lock (initLocker)
                    {
                        if (configuration == null)
                        {
                            configuration = new Configuration();
                            try
                            {
                                JsonTool.PopulateObject(FileTool.ReadAllText(path, encoding), configuration);
                            }
                            catch (Exception)
                            {
                                LoggerHolder.Instance.Error("{LogName}: 以编码{Encoding}读取配置文件{Path}时出错",
                                    LogName, encoding, path);
                            }
                        }
                    }
                }
                return configuration;
            }
        }
        /// <summary>
        /// 将配置保存到文件,路径由Configuration.path指定
        /// </summary>
        public static void Save()
        {
            try
            {
                FileTool.WriteAllText(path, JsonTool.SerializeObject(Instance, formatting: true), encoding);
            }
            catch (Exception)
            {
                LoggerHolder.Instance.Error("{LogName}: 以编码{Encoding}保存配置文件{Path}时出错",
                                    LogName, encoding, path);
            }
        }
        // 配置项

        // 数据库配置
        public string DatabaseType { get; set; } = "Sqlite";
        public string DataSource { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "3306";
        public string User { get; set; } = "root";
        public string Password { get; set; } = "root";
        public string Database { get; set; } = "test";
        public string Charset { get; set; } = "utf8";
        public string Security { get; set; } = "none";
        public string Attachs { get; set; } = @".\data.db";
        public bool Pooling { get; set; } = true;
        public int MinPoolSize { get; set; } = 1;
        // 信誉系统配置
        /// <summary>
        /// 信誉超过(>=)Key,获得Value借书时长(天)
        /// </summary>
        public Dictionary<int,int> CreditReward { get; set; } = new Dictionary<int, int> { { 0,15 }, { 30, 30 }, { 60, 60 } };
        /// <summary>
        /// 还书时间减最后期限间隔天数超过(>=)Key,减少Value信誉值
        /// </summary>
        public Dictionary<int, int> GiveBackPunishment { get; set; } = new Dictionary<int, int> { { 15, 2 } , { 30, 10 }, { 60, 40 }};
        /// <summary>
        /// 最后期限减还书时间间隔天数超过(>=)Key,增加Value信誉值
        /// </summary>
        public Dictionary<int, int> GiveBackReward { get; set; } = new Dictionary<int, int> { { 0, 1 }, { 30, 5 } };
        /// <summary>
        /// 借阅时长超过(>=)这个值才计算信誉值变动(防止无意义反复借阅刷信誉分)
        /// </summary>
        public int CreditBorrowLimit { get; set; } = 2;
        // FreeSql数据库连接字符串Raw
        private static readonly string MySqlConnStr = @"Data Source={0};Port={1};User ID={2};Password={3}; Initial Catalog={4};Charset={5}; SslMode={6};Min pool size={7}";
        private static readonly string PostgreSQLConnStr = @"Host={0};Port={1};Username={2};Password={3}; Database={4};Pooling={5};Minimum Pool Size={6}";
        private static readonly string SqlServerConnStr = @"Data Source={0};Integrated Security={1};Initial Catalog={2};Pooling={3};Min Pool Size={4}";
        private static readonly string OracleConnStr = @"user id={0};password={1}; data source={2};Pooling={3};Min Pool Size={4}";
        private static readonly string SqliteConnStr = @"Data Source={0}; Attachs={1}; Pooling={2};Min Pool Size={3}";
        /// <summary>
        /// FreeSql数据库类型
        /// </summary>
        [JsonIgnore()]
        public DataType SqlDataType
        {
            get
            {
                return (DatabaseType.ToLower()) switch
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
        [JsonIgnore()]
        public string SqlConnectionString
        {
            get
            {
                return (DatabaseType.ToLower()) switch
                {
                    "mysql" => String.Format(MySqlConnStr, DataSource, Port, User, Password, Database, Charset, Security, MinPoolSize),
                    "postgresql" => String.Format(PostgreSQLConnStr, DataSource, Port, User, Password, Database, Pooling, MinPoolSize),
                    "sqlserver" => String.Format(SqlServerConnStr, DataSource, Security, Database, Pooling, MinPoolSize),
                    "oracle" => String.Format(OracleConnStr, User, Password, DataSource, Pooling, MinPoolSize),
                    "sqlite" => String.Format(SqliteConnStr, DataSource, Attachs, Pooling, MinPoolSize),
                    _ => String.Format(SqliteConnStr, DataSource, Attachs, Pooling, MinPoolSize),
                };
            }
        }
    }
}
