using FourOFive.Models.DataBaseModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FourOFive.Services
{
    public interface IBookService
    {
        public Task<int> InStockAsync(IEnumerable<Book> books);
        public Task<int> OutStockAsync(IEnumerable<Book> ids);
        public Task<List<Book>> QueryAsync(Expression<Func<Book, bool>> whereExp = null, int pageIndex = 0, int pageSize = 0);
        public Task<long> CountAsync(Expression<Func<Book, bool>> whereExp = null);
    }
}
