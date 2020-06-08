using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utilities;
using Serilog.Core;
using System;
using System.Text;

namespace LibraryManagementSystem.DAO
{
    public class ConfigDAO
    {
        // 配置文件位置
        private readonly string path;
        // 文件编码
        private readonly Encoding encoding;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        public ConfigDAO(string path, Encoding encoding, Logger logger)
        {
            LogName = GetType().Name;
            this.path = path;
            this.encoding = encoding;
            this.logger = logger;
        }
        public Config LoadConfig(out bool successful)
        {
            Config config = new Config();
            successful = true;
            try
            {
                JsonUtility.PopulateObject(FileUtility.ReadAllText(path, encoding), config);
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "{LogName}: 以编码{Encoding}读取配置文件{Path}时出错",
                    LogName, encoding, path);
                successful = false;
                config = new Config();
            }
            return config;
        }
        public bool Save(Config config)
        {
            bool successful = true;
            try
            {
                FileUtility.WriteAllText(path, JsonUtility.SerializeObject(config, formatting: true), encoding);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 以编码{Encoding}保存配置文件{Path}时出错",
                                    LogName, encoding, path);
                successful = false;
            }
            return successful;
        }
    }
}
