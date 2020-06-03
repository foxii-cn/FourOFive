using LibraryManagementSystem.Config;
using System;
using System.Linq;

namespace LibraryManagementSystem.Service
{
    public static class CreditService
    {
        /// <summary>
        /// 我相信这里用不到这个东西
        /// </summary>
        public static readonly string LogName = "CreditService";
        public static int GetAccreditedDays(int creditValue)
        {
            int[] keys = Configuration.Instance.CreditReward.Keys.ToArray();
            int index = Array.BinarySearch(keys, creditValue);
            index = index < 0 ? ~index - 1 : index;
            if (index < 0)
                return 0;
            return Configuration.Instance.CreditReward[keys[index]];
        }
        public static int GetCreditValue(DateTime borrowTime, DateTime deadlineTime, DateTime giveBackTime, int creditValue)
        {
            if ((giveBackTime - borrowTime).Days < Configuration.Instance.CreditBorrowLimit)
                return creditValue;
            int differDays = (deadlineTime - giveBackTime).Days;
            int[] rewardKeys = Configuration.Instance.GiveBackReward.Keys.ToArray();
            int[] punishmentKeys = Configuration.Instance.GiveBackPunishment.Keys.ToArray();
            int reward = 0;
            int punishment = 0;
            int rewardIndex = Array.BinarySearch(rewardKeys, differDays);
            rewardIndex = rewardIndex < 0 ? ~rewardIndex - 1 : rewardIndex;
            int punishmentIndex = Array.BinarySearch(punishmentKeys, -differDays);
            punishmentIndex = punishmentIndex < 0 ? ~punishmentIndex - 1 : punishmentIndex;
            if (rewardIndex >= 0)
                reward = Configuration.Instance.GiveBackReward[rewardKeys[rewardIndex]];
            if (punishmentIndex >= 0)
                punishment = Configuration.Instance.GiveBackPunishment[punishmentKeys[punishmentIndex]];
            return creditValue + reward - punishment;
        }
    }
}
