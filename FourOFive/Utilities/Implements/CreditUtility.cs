using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FourOFive.Utilities.Implements
{
    public class CreditUtility : ICreditUtility
    {
        private readonly ILogManager logger;
        private readonly IConfigurationManager config;

        private readonly Dictionary<int, int> accredit = new Dictionary<int, int> { { 0, 15 }, { 30, 30 }, { 60, 60 } };
        private readonly Dictionary<int, int> creditPunishment = new Dictionary<int, int> { { 15, 2 }, { 30, 10 }, { 60, 40 } };
        private readonly Dictionary<int, int> creditReward = new Dictionary<int, int> { { 0, 1 }, { 30, 5 } };
        private readonly int changeMinLimit = 2;
        private readonly int initial = 30;

        public CreditUtility(ILogManagerFactory loggerFactory, IConfigurationManagerFactory configFactory)
        {
            logger = loggerFactory.CreateManager<CreditUtility>();
            config = configFactory.CreateManager("Credit");

            accredit = config.Get("AccreditRule", accredit);
            creditPunishment = config.Get("CreditReduceRule", creditPunishment);
            creditReward = config.Get("CreditIncreaseRule", creditReward);
            changeMinLimit = config.Get("ChangeCreditOnlyWhenBorrowTimeOver", changeMinLimit);
            initial = config.Get("InitialCreditValue", initial);
        }
        public int GetAccreditedDays(User user)
        {
            if (user == null)
            {
                return -1;
            }

            int rewardDays = 0;
            int[] keys = accredit.Keys.ToArray();
            int index = Array.BinarySearch(keys, user.CreditValue);
            index = index < 0 ? ~index - 1 : index;
            if (index >= 0)
            {
                rewardDays = accredit[keys[index]];
            }

            return rewardDays;
        }

        public int GetCreditChange(BorrowLog borrowLog)
        {
            DateTime giveBackTime = borrowLog.GiveBack ?? throw new NullReferenceException("图书归还时间为空");
            if ((giveBackTime - borrowLog.CreateTime).Days < changeMinLimit)
            {
                return 0;
            }

            int differDays = (borrowLog.Deadline - giveBackTime).Days;
            int[] rewardKeys = creditReward.Keys.ToArray();
            int[] punishmentKeys = creditPunishment.Keys.ToArray();
            int reward = 0;
            int punishment = 0;
            int rewardIndex = Array.BinarySearch(rewardKeys, differDays);
            rewardIndex = rewardIndex < 0 ? ~rewardIndex - 1 : rewardIndex;
            int punishmentIndex = Array.BinarySearch(punishmentKeys, -differDays);
            punishmentIndex = punishmentIndex < 0 ? ~punishmentIndex - 1 : punishmentIndex;
            if (rewardIndex >= 0)
            {
                reward = creditReward[rewardKeys[rewardIndex]];
            }

            if (punishmentIndex >= 0)
            {
                punishment = creditPunishment[punishmentKeys[punishmentIndex]];
            }

            int creditValueChange = reward - punishment;
            return creditValueChange;
        }

        public int GetInitialCreditValue()
        {
            return initial;
        }
    }
}
