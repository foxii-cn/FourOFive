using LibraryManagementSystem.Models.DataBaseModels;

namespace LibraryManagementSystem.Events
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
