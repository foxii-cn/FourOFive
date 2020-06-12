using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DAO
{
    public class UserDAO : DatabaseDAO<User>
    {
        public UserDAO(IFreeSql sql) : base(sql)
        {
        }
    }
}
