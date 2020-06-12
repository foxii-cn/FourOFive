﻿using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DAO
{
    public class BorrowLogDAO : DatabaseDAO<BorrowLog>
    {
        public BorrowLogDAO(IFreeSql sql) : base(sql)
        {
        }
    }
}
