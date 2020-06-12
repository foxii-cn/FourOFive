using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DAO
{
    public class ISBNInfoDAO : DatabaseDAO<ISBNInfo>
    {
        public ISBNInfoDAO(IFreeSql sql) : base(sql)
        {
        }
    }
}
