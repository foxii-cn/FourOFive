﻿using FreeSql;
using LibraryManagementSystem.LogSys;
using LibraryManagementSystem.Tool;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
        public string DatabaseDataSource { get; set; } = "127.0.0.1";
        public string DatabasePort { get; set; } = "3306";
        public string DatabaseUser { get; set; } = "root";
        public string DatabasePassword { get; set; } = "root";
        public string Database { get; set; } = "test";
        public string DatabaseCharset { get; set; } = "utf8";
        public string DatabaseSecurity { get; set; } = "none";
        public string DatabaseAttachs { get; set; } = @".\data.db";
        public bool DatabasePooling { get; set; } = true;
        public int DatabaseMinPoolSize { get; set; } = 1;
        // 信誉系统配置
        /// <summary>
        /// 信誉超过(>=)Key,获得Value借书时长(天)
        /// </summary>
        public Dictionary<int, int> CreditReward { get; set; } = new Dictionary<int, int> { { 0, 15 }, { 30, 30 }, { 60, 60 } };
        /// <summary>
        /// 还书时间减最后期限间隔天数超过(>=)Key,减少Value信誉值
        /// </summary>
        public Dictionary<int, int> GiveBackPunishment { get; set; } = new Dictionary<int, int> { { 15, 2 }, { 30, 10 }, { 60, 40 } };
        /// <summary>
        /// 最后期限减还书时间间隔天数超过(>=)Key,增加Value信誉值
        /// </summary>
        public Dictionary<int, int> GiveBackReward { get; set; } = new Dictionary<int, int> { { 0, 1 }, { 30, 5 } };
        /// <summary>
        /// 借阅时长超过(>=)这个值才计算信誉值变动(防止无意义反复借阅刷信誉分)
        /// </summary>
        public int CreditBorrowLimit { get; set; } = 2;
        /// <summary>
        /// 会员初始信誉值
        /// </summary>
        public int InitialCreditValue { get; set; } = 30;

        // 密码加密配置
        /// <summary>
        /// 盐长度(Byte)
        /// </summary>
        public int SaltSize { get; set; } = 16;
        /// <summary>
        /// PBKDF2加密算法类型
        /// </summary>
        public string PBKDF2PrfString { get; set; } = "HMACSHA256";
        /// <summary>
        /// PBKDF2加密算法迭代次数
        /// </summary>
        public int PBKDF2IterationTimes { get; set; } = 1000;
        /// <summary>
        /// PBKDF2加密算法生成长度(Byte)
        /// </summary>
        public int PBKDF2SizeRequested { get; set; } = 32;

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
                    "mysql" => String.Format(MySqlConnStr, DatabaseDataSource, DatabasePort, DatabaseUser, DatabasePassword, Database, DatabaseCharset, DatabaseSecurity, DatabaseMinPoolSize),
                    "postgresql" => String.Format(PostgreSQLConnStr, DatabaseDataSource, DatabasePort, DatabaseUser, DatabasePassword, Database, DatabasePooling, DatabaseMinPoolSize),
                    "sqlserver" => String.Format(SqlServerConnStr, DatabaseDataSource, DatabaseSecurity, Database, DatabasePooling, DatabaseMinPoolSize),
                    "oracle" => String.Format(OracleConnStr, DatabaseUser, DatabasePassword, DatabaseDataSource, DatabasePooling, DatabaseMinPoolSize),
                    "sqlite" => String.Format(SqliteConnStr, DatabaseDataSource, DatabaseAttachs, DatabasePooling, DatabaseMinPoolSize),
                    _ => String.Format(SqliteConnStr, DatabaseDataSource, DatabaseAttachs, DatabasePooling, DatabaseMinPoolSize),
                };
            }
        }
        [JsonIgnore()]
        public KeyDerivationPrf PBKDF2Prf
        {
            get
            {
                return (PBKDF2PrfString.ToUpper()) switch
                {
                    "HMACSHA1" => KeyDerivationPrf.HMACSHA1,
                    "HMACSHA256" => KeyDerivationPrf.HMACSHA256,
                    "HMACSHA512" => KeyDerivationPrf.HMACSHA512,
                    _ => KeyDerivationPrf.HMACSHA256
                };
            }
        }
    }
}
