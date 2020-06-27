using FourOFive.Models.DataBaseModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FourOFive.Services
{
    public interface IBorrowService
    {
        public Task<BorrowLog> BorrowBookAsync(User userId, Book bookId);
        public Task<BorrowLog> RevertBookAsync(User userId, Book bookId);
        public Task<List<BorrowLog>> QueryOverTimeLeaseAsync(User userId);
        public Task<List<BorrowLog>> QueryAsync(Expression<Func<BorrowLog, bool>> whereExp = null, int pageIndex = 0, int pageSize = 0);
        public Task<long> CountAsync(Expression<Func<BorrowLog, bool>> whereExp = null);
    }
}
