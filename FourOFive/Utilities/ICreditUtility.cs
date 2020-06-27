using FourOFive.Models.DataBaseModels;

namespace FourOFive.Utilities
{
    public interface ICreditUtility
    {
        public int GetAccreditedDays(User user);
        public int GetCreditChange(BorrowLog borrowLog);
        public int GetInitialCreditValue();
    }
}
