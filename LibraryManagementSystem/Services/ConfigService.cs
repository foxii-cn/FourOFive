using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Models;
using Serilog.Core;
using System;

namespace LibraryManagementSystem.Services
{
    public class ConfigService
    {
        // DAO对象
        private readonly ConfigDAO configDAO;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;
        // 主配置实例
        private Config config;


        public ConfigService(ConfigDAO configDAO, Logger logger)
        {
            LogName = GetType().Name;
            this.configDAO = configDAO;
            this.logger = logger;
        }
        public bool Initialization()
        {
            bool state = true;
            try
            {
                config = configDAO.LoadConfig();
            }
            catch (Exception)
            {
                config = new Config();
                try
                {
                    configDAO.Save(config);
                }
                catch (Exception ex)
                {
                    state = false;
                    logger.Error(ex, "{LogName}: 保存配置文件出错",
                                    LogName);
                }
            }
            return state;
        }
        public Config GetConfig()
        {
            return config;
        }
        public void Close()
        {
            try
            {
                configDAO.Save(config);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 保存配置文件出错",
                                LogName);
            }
        }
    }
}
