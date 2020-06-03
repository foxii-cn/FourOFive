using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Entity;
using LibraryManagementSystem.LogSys;
using LibraryManagementSystem.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem.Controller
{
    public static class BorrowController
    {
        public static readonly string LogName = "BorrowController";
        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="user">借书的会员</param>
        /// <param name="book">要借的书</param>
        public static void BorrowBook(string userId, string bookId)
        {
            if (userId == null || bookId == null)
                throw new NullReferenceException("借阅人或借阅目标ID为空");
            try
            {
                Sql.Instance.Transaction(() =>
                {
                    DatabaseService<Book>.ForUpdate(bookId);
                    Book bSnap = DatabaseService<Book>.Query(bookId).FirstOrDefault();
                    if (bSnap == null)
                        throw new Exception("借阅目标不合法");
                    if (bSnap.Margin <= 0)
                        throw new Exception("借阅目标数量不足");
                    User uSnap = DatabaseService<User>.Query(userId).FirstOrDefault();
                    if (uSnap == null)
                        throw new Exception("借阅人不合法");
                    string leaseConditionString = String.Format(@"BookId='{0}' and UserId='{1}' and GiveBack is NULL",
                        bSnap.Id, uSnap.Id);
                    List<LeaseLog> leaseLogs = DatabaseService<LeaseLog>.QuerySql(leaseConditionString);
                    if (leaseLogs.Count > 0)
                        throw new Exception("已有未归还的同名同书借阅记录");
                    int accreditedDays = CreditService.GetAccreditedDays(uSnap.CreditValue);
                    if (accreditedDays == 0)
                        throw new Exception("借阅人信誉过低");
                    bSnap.Margin--;
                    DatabaseService<Book>.Update(bSnap, String.Format(@"Margin={0}", bSnap.Margin));
                    LeaseLog leaseLog = new LeaseLog { BookId = bSnap.Id, UserId = uSnap.Id, Deadline = DateTime.Today.AddDays(accreditedDays + 1) };
                    DatabaseService<LeaseLog>.Create(leaseLog);
                });
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: {UserId}借书{BookId}失败({ExceptionMessage})",
                                    LogName, userId, bookId, ex.Message);
                throw;
            }
        }
        public static void RevertBook(string userId, string bookId)
        {
            if (userId == null || bookId == null)
                throw new NullReferenceException("借阅人或借阅目标ID为空");
            try
            {
                Sql.Instance.Transaction(() =>
                {
                    DatabaseService<Book>.ForUpdate(bookId);
                    DatabaseService<User>.ForUpdate(userId);
                    Book bSnap = DatabaseService<Book>.Query(bookId).FirstOrDefault();
                    if (bSnap == null)
                        throw new Exception("借阅目标不合法");
                    User uSnap = DatabaseService<User>.Query(userId).FirstOrDefault();
                    if (uSnap == null)
                        throw new Exception("借阅人不合法");
                    string leaseConditionString = String.Format(@"BookId='{0}' and UserId='{1}' and GiveBack is NULL",
                        bSnap.Id, uSnap.Id);
                    DatabaseService<LeaseLog>.ForUpdateSql(leaseConditionString);
                    List<LeaseLog> leaseLogs = DatabaseService<LeaseLog>.QuerySql(leaseConditionString);
                    if (leaseLogs.Count == 0)
                        throw new Exception("无未归还的借阅记录");
                    LeaseLog leaseLog = leaseLogs.OrderBy(l => l.CreateTime).FirstOrDefault();
                    DateTime giveBack = DateTime.Now;
                    bSnap.Margin++;
                    leaseLog.GiveBack = giveBack;
                    uSnap.CreditValue = CreditService.GetCreditValue(leaseLog.CreateTime, (DateTime)leaseLog.Deadline, giveBack, uSnap.CreditValue);
                    DatabaseService<Book>.Update(bSnap, String.Format(@"Margin={0}", bSnap.Margin));
                    DatabaseService<User>.Update(uSnap, String.Format(@"CreditValue={0}", uSnap.CreditValue));
                    DatabaseService<LeaseLog>.Update(leaseLog, String.Format(@"GiveBack='{0}'", leaseLog.GiveBack));
                });
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: {UserId}还书{BookId}失败({ExceptionMessage})",
                                    LogName, userId, bookId, ex.Message);
                throw;
            }
        }
        public static List<LeaseLog> TardyLease(string userId = null)
        {
            string leaseConditionString = @"GiveBack is NULL";
            if (userId != null)
                leaseConditionString = String.Format(@"{0} and UserId='{1}'", leaseConditionString, userId);
            List<LeaseLog> leaseLogs;
            try
            {
                leaseLogs = DatabaseService<LeaseLog>.QuerySql(leaseConditionString);
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning("{LogName}: 查询未还记录失败({ExceptionMessage})",
                                    LogName, ex.Message);
                throw;
            }
            return leaseLogs;
        }
    }
}
