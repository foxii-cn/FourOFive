using LibraryManagementSystem.Models;
using Serilog.Core;

namespace LibraryManagementSystem.DAO
{
    public class BorrowLogDAO : DatabaseDAO<BorrowLog>
    {
        public BorrowLogDAO(IFreeSql sql, Logger logger) : base(sql, logger)
        {
        }
    }
}
