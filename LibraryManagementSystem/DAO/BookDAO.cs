using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DAO
{
    public class BookDAO : DatabaseDAO<Book>
    {
        public BookDAO(IFreeSql sql) : base(sql)
        {
        }
    }
}
