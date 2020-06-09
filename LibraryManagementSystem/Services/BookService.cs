using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Models;
using Serilog.Core;
using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Services
{
    public class BookService
    {
        // DAO对象
        private readonly BookDAO bookDAO;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        public BookService(BookDAO bookDAO, Logger logger)
        {
            LogName = GetType().Name;
            this.bookDAO = bookDAO;
            this.logger = logger;
        }
        public Book InStock(string title, string author = null, string publishingHouse = null, int sum = 0, string position = null)
        {
            Book book = new Book
            {
                Title = title,
                Author = author,
                PublishingHouse = publishingHouse,
                Margin = sum,
                Sum = sum,
                Position = position
            };
            int affectedRows;
            try
            {
                affectedRows = bookDAO.Create(book);
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: 书{Book}入库失败({ExceptionMessage})",
                                    LogName, book, ex.Message);
                throw;
            }
            if (affectedRows == 1)
                return book;
            else
                return null;
        }
        public int OutStock(params Guid[] bookIds)
        {
            int affectedRows;
            try
            {
                affectedRows = bookDAO.Delete(bookIds);
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: 书出库失败({ExceptionMessage})",
                                    LogName, ex.Message);
                throw;
            }
            return affectedRows;
        }
        public List<Book> BookQuery(string condition, int pageIndex, int pageSize, out long count, object parms)
        {
            List<Book> books;
            try
            {
                books = bookDAO.QuerySql(condition, pageIndex, pageSize, out count, parms);
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: 书查询失败({ExceptionMessage})",
                                    LogName, ex.Message);
                throw;
            }
            return books;
        }
    }
}
