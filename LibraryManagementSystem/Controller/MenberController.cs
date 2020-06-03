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

        public static Member Register(string userName, string password, string name, string nationalIdentificationNumber)
        {
            byte[] vs = EncryptTool.NewSalt;
            Member member = new Member
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
                affectedRows = DatabaseService<Member>.Create(member);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: 用户{Member}注册失败({ExceptionMessage})",
                                    LogName, member, ex.Message);
                throw;
            }
            if (affectedRows == 1)
                return member;
            else
                return null;
        }
        public static bool LogIn(string userName, string password)
        {
            string memberConditionString = String.Format(@"UserName='{0}'", userName);
            Member member;
            try
            {
                member = DatabaseService<Member>.QuerySql(memberConditionString).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: 用户{UserName}查询失败({ExceptionMessage})",
                                    LogName, userName, ex.Message);
                throw;
            }
            if (member != null && EncryptTool.HashEncrypt(password, Convert.FromBase64String(member.Salt)) == member.Password)
                return true;
            else
                return false;
        }
    }
}
