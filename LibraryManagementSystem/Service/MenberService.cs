using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Model;
using Serilog.Core;
using System;
using System.Linq;

namespace LibraryManagementSystem.Service
{
    public class MenberService
    {
        // DAO对象
        private readonly UserDAO userDAO;
        // 信誉服务对象
        private readonly CreditService creditService;
        // 加密服务对象
        private readonly EncryptService encryptService;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        public MenberService(UserDAO userDAO, CreditService creditService, EncryptService encryptService, Logger logger)
        {
            LogName = GetType().Name;
            this.userDAO = userDAO;
            this.creditService = creditService;
            this.encryptService = encryptService;
            this.logger = logger;
        }
        public User Register(string userName, string password, string name, string nationalIdentificationNumber)
        {
            byte[] vs = encryptService.CreateNewSalt();
            User user = new User
            {
                UserName = userName,
                Salt = Convert.ToBase64String(vs),
                Password = encryptService.HashEncrypt(password, vs),
                Name = name,
                NationalIdentificationNumber = nationalIdentificationNumber,
                CreditValue = creditService.GetInitialCreditValue()
            };
            int affectedRows;
            try
            {
                affectedRows = userDAO.Create(user);
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: 用户{User}注册失败({ExceptionMessage})",
                                    LogName, user, ex.Message);
                throw;
            }
            if (affectedRows == 1)
                return user;
            else
                return null;
        }
        public bool LogIn(string userName, string password)
        {
            string userConditionString = string.Format(@"UserName='{0}'", userName);
            User user;
            try
            {
                user = userDAO.QuerySql(userConditionString).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: 用户{UserName}查询失败({ExceptionMessage})",
                                    LogName, userName, ex.Message);
                throw;
            }
            if (user != null && encryptService.HashEncrypt(password, Convert.FromBase64String(user.Salt)) == user.Password)
                return true;
            else
                return false;
        }
    }
}
