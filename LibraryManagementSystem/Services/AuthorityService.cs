using LibraryManagementSystem.Models;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    public class AuthorityService
    {
        // 配置对象
        private readonly Config config;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        public AuthorityService(Config config, Logger logger)
        {
            LogName = GetType().Name;
            this.config = config;
            this.logger = logger;
        }
        public bool IsAdministrator(Guid guid,string userName,int authority)
        {
            if (guid!=null&&!String.IsNullOrEmpty(userName)&&authority >= config.AdministratorAuthority)
            {
                logger.Information("{LogName}: 用户{UserName}({Guid})通过了管理员认证",
                                    LogName, userName, guid);
                return true;
            }
            else return false;
        }
        public int GetInitialUserAuthority()
        {
            return config.InitialUserAuthority;
        }
    }
}
