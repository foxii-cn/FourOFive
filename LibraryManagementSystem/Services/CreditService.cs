using LibraryManagementSystem.Models;
using Serilog.Core;
using System;
using System.Linq;

namespace LibraryManagementSystem.Services
{
    public class CreditService
    {
        // 配置对象
        private readonly Config config;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        public CreditService(Config config, Logger logger)
        {
            LogName = GetType().Name;
            this.config = config;
            this.logger = logger;
        }
        public int GetAccreditedDays(int creditValue)
        {
            int rewardDays = 0;
            try
            {
                int[] keys = config.CreditReward.Keys.ToArray();
                int index = Array.BinarySearch(keys, creditValue);
                index = index < 0 ? ~index - 1 : index;
                if (index >= 0)
                    rewardDays = config.CreditReward[keys[index]];
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 计算可借阅天数出错",
                                    LogName);
            }
            return rewardDays;
        }
        public int GetCreditChange(DateTime borrowTime, DateTime deadlineTime, DateTime giveBackTime, int creditValue)
        {
            int creditValueChange = 0;
            try
            {
                if ((giveBackTime - borrowTime).Days < config.CreditBorrowLimit)
                    return 0;
                int differDays = (deadlineTime - giveBackTime).Days;
                int[] rewardKeys = config.GiveBackReward.Keys.ToArray();
                int[] punishmentKeys = config.GiveBackPunishment.Keys.ToArray();
                int reward = 0;
                int punishment = 0;
                int rewardIndex = Array.BinarySearch(rewardKeys, differDays);
                rewardIndex = rewardIndex < 0 ? ~rewardIndex - 1 : rewardIndex;
                int punishmentIndex = Array.BinarySearch(punishmentKeys, -differDays);
                punishmentIndex = punishmentIndex < 0 ? ~punishmentIndex - 1 : punishmentIndex;
                if (rewardIndex >= 0)
                    reward = config.GiveBackReward[rewardKeys[rewardIndex]];
                if (punishmentIndex >= 0)
                    punishment = config.GiveBackPunishment[punishmentKeys[punishmentIndex]];
                creditValueChange = reward - punishment;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 计算信誉值变化出错",
                                    LogName);
            }
            return creditValueChange;
        }
        public int GetInitialCreditValue()
        {
            return config.InitialCreditValue;
        }
    }
}
