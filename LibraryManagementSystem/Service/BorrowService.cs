using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Model;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem.Service
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

        public void BorrowBook(string userId, string bookId)
        {
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
                    string leaseConditionString = string.Format(@"BookId='{0}' and UserId='{1}' and GiveBack is NULL",
                        bSnap.Id, uSnap.Id);
                    List<BorrowLog> leaseLogs = borrowLogDAO.QuerySql(leaseConditionString);
                    if (leaseLogs.Count > 0)
                        throw new Exception("已有未归还的同名同书借阅记录");
                    int accreditedDays = creditService.GetAccreditedDays(uSnap.CreditValue);
                    if (accreditedDays == 0)
                        throw new Exception("借阅人信誉过低");
                    bSnap.Margin--;
                    bookDAO.Update(bSnap, string.Format(@"Margin={0}", bSnap.Margin));
                    BorrowLog leaseLog = new BorrowLog { BookId = bSnap.Id, UserId = uSnap.Id, Deadline = DateTime.Today.AddDays(accreditedDays + 1) };
                    borrowLogDAO.Create(leaseLog);
                });
            }
            catch (Exception ex)
            {
                logger.Warning("{LogName}: {UserId}借书{BookId}失败({ExceptionMessage})",
                                    LogName, userId, bookId, ex.Message);
                throw;
            }
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
                    string leaseConditionString = string.Format(@"BookId='{0}' and UserId='{1}' and GiveBack is NULL",
                        bSnap.Id, uSnap.Id);
                    borrowLogDAO.ForUpdateSql(leaseConditionString);
                    List<BorrowLog> leaseLogs = borrowLogDAO.QuerySql(leaseConditionString);
                    if (leaseLogs.Count == 0)
                        throw new Exception("无未归还的借阅记录");
                    BorrowLog leaseLog = leaseLogs.OrderBy(l => l.CreateTime).FirstOrDefault();
                    DateTime giveBack = DateTime.Now;
                    bSnap.Margin++;
                    leaseLog.GiveBack = giveBack;
                    uSnap.CreditValue = creditService.GetCreditValue(leaseLog.CreateTime, (DateTime)leaseLog.Deadline, giveBack, uSnap.CreditValue);
                    bookDAO.Update(bSnap, string.Format(@"Margin={0}", bSnap.Margin));
                    userDAO.Update(uSnap, string.Format(@"CreditValue={0}", uSnap.CreditValue));
                    borrowLogDAO.Update(leaseLog, string.Format(@"GiveBack='{0}'", leaseLog.GiveBack));
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
            if (userId != null)
                leaseConditionString = string.Format(@"{0} and UserId='{1}'", leaseConditionString, userId);
            List<BorrowLog> leaseLogs;
            try
            {
                leaseLogs = borrowLogDAO.QuerySql(leaseConditionString);
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
