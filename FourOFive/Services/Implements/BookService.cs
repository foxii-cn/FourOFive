using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FourOFive.Services
{
    public class BookService : IBookService
    {
        private readonly IDatabaseModelManager database;
        private readonly ILogManager logger;

        public BookService(IDatabaseModelManager database, ILogManagerFactory loggerFactory)
        {
            this.database = database;
            logger = loggerFactory.CreateManager<BookService>();
        }

        public async Task<long> CountAsync(Expression<Func<Book, bool>> whereExp = null)
        {
            return await database.CountAsync(whereExp);
        }

        public async Task<int> InStockAsync(IEnumerable<Book> books)
        {
            int rows;
            using IDatabaseTransactionManager transaction = database.StartTransaction();
            try
            {
                rows = await database.InsertAsync(books, transaction.GetTransaction());
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            return rows;
        }

        public async Task<int> OutStockAsync(IEnumerable<Book> ids)
        {
            int rows;
            using IDatabaseTransactionManager transaction = database.StartTransaction();
            try
            {
                rows = await database.DeleteAsync<Book, Book>(ids: ids, transaction: transaction.GetTransaction());
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            return rows;
        }

        public async Task<List<Book>> QueryAsync(Expression<Func<Book, bool>> whereExp = null, int pageIndex = 0, int pageSize = 0)
        {
            return await database.SelectAsync<Book, Book>(whereExp, pageIndex: pageIndex, pageSize: pageSize);
        }
    }
}
