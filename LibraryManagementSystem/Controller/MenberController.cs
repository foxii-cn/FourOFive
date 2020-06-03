using LibraryManagementSystem.Config;
using LibraryManagementSystem.Entity;
using LibraryManagementSystem.LogSys;
using LibraryManagementSystem.Service;
using LibraryManagementSystem.Tool;
using System;
using System.Linq;

namespace LibraryManagementSystem.Controller
{
    public static class MenberController
    {
        public static readonly string LogName = "MenberController";

        public static User Register(string userName, string password, string name, string nationalIdentificationNumber)
        {
            byte[] vs = EncryptTool.NewSalt;
            User user = new User
            {
                UserName = userName,
                Salt = Convert.ToBase64String(vs),
                Password = EncryptTool.HashEncrypt(password, vs),
                Name = name,
                NationalIdentificationNumber = nationalIdentificationNumber,
                CreditValue = Configuration.Instance.InitialCreditValue
            };
            int affectedRows;
            try
            {
                affectedRows = DatabaseService<User>.Create(user);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: 用户{User}注册失败({ExceptionMessage})",
                                    LogName, user, ex.Message);
                throw;
            }
            if (affectedRows == 1)
                return user;
            else
                return null;
        }
        public static bool LogIn(string userName, string password)
        {
            string userConditionString = String.Format(@"UserName='{0}'", userName);
            User user;
            try
            {
                user = DatabaseService<User>.QuerySql(userConditionString).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: 用户{UserName}查询失败({ExceptionMessage})",
                                    LogName, userName, ex.Message);
                throw;
            }
            if (user != null && EncryptTool.HashEncrypt(password, Convert.FromBase64String(user.Salt)) == user.Password)
                return true;
            else
                return false;
        }
    }
}
