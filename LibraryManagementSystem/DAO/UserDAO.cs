using LibraryManagementSystem.Models;
using Serilog.Core;

namespace LibraryManagementSystem.DAO
{
    public class UserDAO : DatabaseDAO<User>
    {
        public UserDAO(IFreeSql sql, Logger logger) : base(sql, logger)
        {
        }
    }
}
