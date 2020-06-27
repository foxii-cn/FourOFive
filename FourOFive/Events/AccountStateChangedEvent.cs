using FourOFive.Models.DataBaseModels;

namespace FourOFive.Events
{
    public class AccountStateChangedEvent
    {
        public User Account { get; }
        public AccountStateChangedEvent(User account)
        {
            Account = account;
        }
    }
}
