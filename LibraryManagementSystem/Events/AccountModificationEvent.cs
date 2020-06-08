using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Events
{
    public class AccountModificationEvent
    {
        public User Account { get; private set; }
        public AccountModificationEvent(User account)
        {
            Account = account;
        }
    }
}
