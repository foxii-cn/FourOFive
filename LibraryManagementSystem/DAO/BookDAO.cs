using LibraryManagementSystem.Model;
using Serilog.Core;

namespace LibraryManagementSystem.DAO
{
    public class BookDAO : DatabaseDAO<Book>
    {
        public BookDAO(IFreeSql sql, Logger logger) : base(sql, logger)
        {
        }
    }
}
