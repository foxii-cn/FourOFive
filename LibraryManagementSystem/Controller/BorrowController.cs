using LibraryManagementSystem.DAO;
using LibraryManagementSystem.Entity;
using LibraryManagementSystem.LogSys;
using LibraryManagementSystem.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagementSystem.Controller
{
    public static class BorrowController
    {
        public static readonly string LogName = "BorrowController";
        /// <summary>
        /// 借书
        /// </summary>
        /// <param name="member">借书的会员</param>
        /// <param name="book">要借的书</param>
        public static void BorrowBook(Member member, Book book)
        {
            Member mSnap = BasicEntity.Copy(member);
            Book bSnap = BasicEntity.Copy(book);
            if (mSnap == null || bSnap == null)
                throw new NullReferenceException("借阅人或借阅目标对象为空");
            try
            {
                Sql.Instance.Transaction(() =>
                {
                    DatabaseService<Book>.ForUpdate(bSnap);
                    bSnap = DatabaseService<Book>.Query(bSnap).FirstOrDefault();
                    if (bSnap == null)
                        throw new Exception("借阅目标不合法");
                    if (bSnap.Margin <= 0)
                        throw new Exception("借阅目标数量不足");
                    mSnap = DatabaseService<Member>.Query(mSnap).FirstOrDefault();
                    if (mSnap == null)
                        throw new Exception("借阅人不合法");
                    string leaseConditionString = String.Format(@"BookId='{0}' and MemberId='{1}' and GiveBack is NULL",
                        bSnap.Id, mSnap.Id);
                    List<LeaseLog> leaseLogs = DatabaseService<LeaseLog>.Query(leaseConditionString);
                    if (leaseLogs.Count > 0)
                        throw new Exception("已有未归还的同名同书借阅记录");
                    int accreditedDays = CreditService.GetAccreditedDays(mSnap.CreditValue);
                    if (accreditedDays == 0)
                        throw new Exception("借阅人信誉过低");
                    bSnap.Margin--;
                    DatabaseService<Book>.Update(new string[] { "Margin" }, bSnap);
                    LeaseLog leaseLog = new LeaseLog { BookId = bSnap.Id, MemberId = mSnap.Id, Deadline = DateTime.Today.AddDays(accreditedDays + 1) };
                    DatabaseService<LeaseLog>.Create(leaseLog);
                });
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning(ex, "{LogName}: {MemberId}借书{BookId}失败",
                                    LogName, member.Id, book.Id);
                throw;
            }
        }
        public static void RevertBook(Member member, Book book)
        {
            Member mSnap = BasicEntity.Copy(member);
            Book bSnap = BasicEntity.Copy(book);
            if (mSnap == null || bSnap == null)
                throw new NullReferenceException("借阅人或借阅目标对象为空");
            try
            {
                Sql.Instance.Transaction(() =>
                {
                    DatabaseService<Book>.ForUpdate(bSnap);
                    DatabaseService<Member>.ForUpdate(mSnap);
                    bSnap = DatabaseService<Book>.Query(bSnap).FirstOrDefault();
                    if (bSnap == null)
                        throw new Exception("借阅目标不合法");
                    mSnap = DatabaseService<Member>.Query(mSnap).FirstOrDefault();
                    if (mSnap == null)
                        throw new Exception("借阅人不合法");
                    string leaseConditionString = String.Format(@"BookId='{0}' and MemberId='{1}' and GiveBack is NULL",
                        bSnap.Id, mSnap.Id);
                    DatabaseService<LeaseLog>.ForUpdate(leaseConditionString);
                    List<LeaseLog> leaseLogs = DatabaseService<LeaseLog>.Query(leaseConditionString);
                    if (leaseLogs.Count == 0)
                        throw new Exception("无未归还的借阅记录");
                    LeaseLog leaseLog = leaseLogs.OrderBy(l => l.CreateTime).FirstOrDefault();
                    DateTime giveBack = DateTime.Now;
                    bSnap.Margin++;
                    leaseLog.GiveBack = giveBack;
                    mSnap.CreditValue = CreditService.GetCreditValue(leaseLog.CreateTime, (DateTime)leaseLog.Deadline, giveBack, mSnap.CreditValue);
                    DatabaseService<Book>.Update(new string[] { "Margin" }, bSnap);
                    DatabaseService<Member>.Update(new string[] { "CreditValue" }, mSnap);
                    DatabaseService<LeaseLog>.Update(new string[] { "GiveBack" }, leaseLog);
                });
            }
            catch (Exception ex)
            {
                LoggerHolder.Instance.Warning(ex, "{LogName}: {MemberId}还书{BookId}失败",
                                    LogName, member.Id, book.Id);
                throw;
            }
        }
    }
}
