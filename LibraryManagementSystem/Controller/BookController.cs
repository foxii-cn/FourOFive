using LibraryManagementSystem.Entity;
using LibraryManagementSystem.LogSys;
using LibraryManagementSystem.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Controller
{
    public static class BookController
    {
        public static readonly string LogName = "BookController";
        public static Book InStock(string title, string author=null, string publishingHouse=null, int sum = 0, string position = null)
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
                affectedRows=DatabaseService<Book>.Create(book);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: 书{Book}入库失败({ExceptionMessage})",
                                    LogName, book,ex.Message);
                throw;
            }
            if (affectedRows == 1)
                return book;
            else
                return null;
        }
        public static int OutStock(params Book[]books)
        {
            int affectedRows;
            try
            {
                affectedRows=DatabaseService<Book>.Delete(books);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: 书出库失败({ExceptionMessage})",
                                    LogName,ex.Message);
                throw;
            }
            return affectedRows;
        }
        public static List<Book> BookQuery(string condition, int pageIndex, int pageSize, out long count)
        {
            List<Book> books;
            try
            {
                books=DatabaseService<Book>.Query(condition, pageIndex, pageSize, out count);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: 书查询失败({ExceptionMessage})",
                                    LogName, ex.Message);
                throw;
            }
            return books;
        }
    }
}
