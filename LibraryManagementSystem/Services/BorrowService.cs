using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Models;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public int BorrowBook(string userId, string bookId)
        {
            int accreditedDays = 0;
            if (userId == null || bookId == null)
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
        public void RevertBook(string userId, string bookId)
        {
            if (userId == null || bookId == null)
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
                    uSnap.CreditValue = creditService.GetCreditValue(leaseLog.CreateTime, (DateTime)leaseLog.Deadline, giveBack, uSnap.CreditValue);
                    bookDAO.Update(bSnap, @"Margin=@Margin", new { bSnap.Margin });
                    userDAO.Update(uSnap, @"CreditValue=@CreditValue", new { uSnap.CreditValue });
                    borrowLogDAO.Update(leaseLog, @"GiveBack=@GiveBack", new { leaseLog.GiveBack });
                });
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: {UserId}还书{BookId}失败({ExceptionMessage})",
                                    LogName, userId, bookId, ex.Message);
                throw;
            }
        }
        public List<BorrowLog> TardyLease(string userId = null)
        {
            string leaseConditionString = @"GiveBack is NULL";
            object leaseParms = null;
            if (userId != null)
            {
                leaseConditionString = @"GiveBack is NULL and UserId=@UserId";
                leaseParms = new { UserId = userId };
            }
            List<BorrowLog> leaseLogs;
            try
            {
                leaseLogs = borrowLogDAO.QuerySql(leaseConditionString, leaseParms);
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: 查询未还记录失败({ExceptionMessage})",
                                    LogName, ex.Message);
                throw;
            }
            return leaseLogs;
        }
    }
}
