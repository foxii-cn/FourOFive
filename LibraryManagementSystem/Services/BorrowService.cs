using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Models;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Services
{
    public class BorrowService
    {
        // DAO对象
        private readonly BookDAO bookDAO;
        private readonly UserDAO userDAO;
        private readonly BorrowLogDAO borrowLogDAO;
        // 信誉服务对象
        private readonly CreditService creditService;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        public BorrowService(BookDAO bookDAO, UserDAO userDAO, BorrowLogDAO borrowLogDAO, CreditService creditService, Logger logger)
        {
            LogName = GetType().Name;
            this.bookDAO = bookDAO;
            this.userDAO = userDAO;
            this.borrowLogDAO = borrowLogDAO;
            this.creditService = creditService;
            this.logger = logger;
        }

        public int BorrowBook(Guid userId, Guid bookId)
        {
            int accreditedDays = 0;
            if (userId == default || bookId == default)
                throw new NullReferenceException("借阅人或借阅目标ID为空");
            try
            {
                borrowLogDAO.Transaction(() =>
                {
                    bookDAO.ForUpdate(bookId);
                    Book bSnap = bookDAO.Query(bookId).FirstOrDefault();
                    if (bSnap == null)
                        throw new Exception("借阅目标不合法");
                    if (bSnap.Margin <= 0)
                        throw new Exception("借阅目标数量不足");
                    User uSnap = userDAO.Query(userId).FirstOrDefault();
                    if (uSnap == null)
                        throw new Exception("借阅人不合法");
                    List<BorrowLog> leaseLogs = borrowLogDAO.QuerySql(@"BookId=@BookId and UserId=@UserId and GiveBack is NULL",
                       new { BookId = bSnap.Id, UserId = uSnap.Id });
                    if (leaseLogs.Count > 0)
                        throw new Exception("已有未归还的同名同书借阅记录");
                    accreditedDays = creditService.GetAccreditedDays(uSnap.CreditValue) + 1;
                    if (accreditedDays == 0)
                        throw new Exception("借阅人信誉过低");
                    bSnap.Margin--;
                    if (bookDAO.Update(bSnap, @"Margin=@Margin", new { bSnap.Margin }) == 0)
                        throw new Exception("更新书本库存失败");
                    BorrowLog leaseLog = new BorrowLog { BookId = bSnap.Id, UserId = uSnap.Id, Deadline = DateTime.Today.AddDays(accreditedDays) };
                    if (borrowLogDAO.Create(leaseLog) == 0)
                        throw new Exception("创建借阅记录失败");
                });
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: {UserId}借书{BookId}失败({ExceptionMessage})",
                                    LogName, userId, bookId, ex.Message);
                throw;
            }
            return accreditedDays;
        }
        public int RevertBook(Guid userId, Guid bookId)
        {
            int creditChange = 0;
            if (userId == default || bookId == default)
                throw new NullReferenceException("借阅人或借阅目标ID为空");
            try
            {
                borrowLogDAO.Transaction(() =>
                {
                    bookDAO.ForUpdate(bookId);
                    userDAO.ForUpdate(userId);
                    Book bSnap = bookDAO.Query(bookId).FirstOrDefault();
                    if (bSnap == null)
                        throw new Exception("借阅目标不合法");
                    User uSnap = userDAO.Query(userId).FirstOrDefault();
                    if (uSnap == null)
                        throw new Exception("借阅人不合法");
                    string leaseConditionString = @"BookId=@BookId and UserId=@UserId and GiveBack is NULL";
                    object leaseParms = new { BookId = bSnap.Id, UserId = uSnap.Id };
                    borrowLogDAO.ForUpdateSql(leaseConditionString, leaseParms);
                    List<BorrowLog> leaseLogs = borrowLogDAO.QuerySql(leaseConditionString, leaseParms);
                    if (leaseLogs.Count == 0)
                        throw new Exception("无未归还的借阅记录");
                    BorrowLog leaseLog = leaseLogs.OrderBy(l => l.CreateTime).FirstOrDefault();
                    DateTime giveBack = DateTime.Now;
                    bSnap.Margin++;
                    leaseLog.GiveBack = giveBack;
                    creditChange = creditService.GetCreditChange(leaseLog.CreateTime, (DateTime)leaseLog.Deadline, giveBack, uSnap.CreditValue);
                    uSnap.CreditValue += creditChange;
                    if (bookDAO.Update(bSnap, @"Margin=@Margin", new { bSnap.Margin }) < 1)
                        throw new Exception("更新书本信息失败");
                    if (userDAO.Update(uSnap, @"CreditValue=@CreditValue", new { uSnap.CreditValue }) < 1)
                        throw new Exception("更新会员信息失败");
                    if (borrowLogDAO.Update(leaseLog, @"GiveBack=@GiveBack,CreditValueHistory=@CreditValue", new { leaseLog.GiveBack, uSnap.CreditValue }) < 1)
                        throw new Exception("更新借阅记录失败");
                });
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: {UserId}还书{BookId}失败({ExceptionMessage})",
                                    LogName, userId, bookId, ex.Message);
                throw;
            }
            return creditChange;
        }
        public List<BorrowLog> TardyLease(Guid userId = default)
        {
            Expression<Func<BorrowLog, bool>> condition;
            if (userId != default)
                condition = bl => bl.GiveBack == null && bl.UserId == userId;
            else
                condition = bl => bl.GiveBack == null;
            List<BorrowLog> leaseLogs;
            try
            {
                leaseLogs = borrowLogDAO.QueryLambda(condition, bl => bl.Book);
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: 查询未还记录失败({ExceptionMessage})",
                                    LogName, ex.Message);
                throw;
            }
            return leaseLogs;
        }
        public List<BorrowLog> BorrowLogQuery(int pageIndex, int pageSize, out long count, Guid userId = default)
        {
            List<BorrowLog> borrowLogs;
            Expression<Func<BorrowLog, bool>> condition;
            if (userId == default)
                condition = bl => true;
            else condition = bl => bl.UserId == userId;
            try
            {
                borrowLogs = borrowLogDAO.QueryLambda(condition, pageIndex, pageSize, out count, bl => bl.Book);
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: 借阅记录查询失败({ExceptionMessage})",
                                    LogName, ex.Message);
                throw;
            }
            return borrowLogs;
        }
    }
}
