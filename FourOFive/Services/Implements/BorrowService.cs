using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using FourOFive.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FourOFive.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly ICreditUtility credit;
        private readonly IDatabaseModelManager database;
        private readonly ILogManager logger;

        public BorrowService(ICreditUtility credit, IDatabaseModelManager database, ILogManagerFactory loggerFactory)
        {
            this.credit = credit;
            this.database = database;
            logger = loggerFactory.CreateManager<BorrowService>();
        }

        public async Task<BorrowLog> BorrowBookAsync(User userId, Book bookId)
        {
            BorrowLog leaseLog = null;
            Book bCheck = (await database.SelectAsync<Book, Book>(b => b.Id == bookId.Id)).FirstOrDefault();
            User uCheck = (await database.SelectAsync<User, User>(u => u.Id == userId.Id)).FirstOrDefault();
            _= (await database.SelectAsync<BorrowLog, BorrowLog>(null, pageIndex: 1, pageSize: 1)).FirstOrDefault();
            if(bCheck == null||uCheck==null)
                throw new Exception("借阅人或目标不合法");
            using IDatabaseTransactionManager transaction = database.StartTransaction();
            try
            {
                await database.ForUpdateAsync<Book, Book>(b => b.Id == bookId.Id, transaction: transaction.GetTransaction());
                Book bSnap = (await database.SelectAsync<Book, Book>(b => b.Id == bookId.Id, transaction: transaction.GetTransaction())).FirstOrDefault();
                if (bSnap == null)
                {
                    throw new Exception("借阅目标不合法");
                }

                if (bSnap.Margin <= 0)
                {
                    throw new Exception("借阅目标数量不足");
                }

                User uSnap = (await database.SelectAsync<User, User>(u => u.Id == userId.Id, transaction: transaction.GetTransaction())).FirstOrDefault();
                if (uSnap == null)
                {
                    throw new Exception("借阅人不合法");
                }

                List<BorrowLog> leaseLogs = await database.SelectAsync<BorrowLog, BorrowLog>(bl => bl.BookId == bookId.Id && bl.UserId == userId.Id && bl.GiveBack == null, transaction: transaction.GetTransaction());
                if (leaseLogs.Count > 0)
                {
                    throw new Exception("已有未归还的同名同书借阅记录");
                }

                int accreditedDays = credit.GetAccreditedDays(uSnap);
                if (accreditedDays == 0)
                {
                    throw new Exception("借阅人信誉过低");
                }

                if (await database.UpdateAsync<Book, int>(book => book.Margin - 1, book => book.Id == bSnap.Id, transaction: transaction.GetTransaction()) == 0)
                {
                    throw new Exception("更新书本库存失败");
                }

                leaseLog = new BorrowLog { BookId = bSnap.Id, UserId = uSnap.Id, Deadline = DateTime.Today.AddDays(accreditedDays) };
                if (await database.InsertAsync(new BorrowLog[] { leaseLog }, transaction: transaction.GetTransaction()) == 0)
                {
                    throw new Exception("创建借阅记录失败");
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            return leaseLog;
        }
        public async Task<BorrowLog> RevertBookAsync(User userId, Book bookId)
        {
            BorrowLog leaseLog = null;
            BorrowLog leaseLogCheck = (await database.SelectAsync<BorrowLog, BorrowLog>(bl => bl.User.Id == userId.Id && bl.Book.Id == bookId.Id && bl.GiveBack == null)).FirstOrDefault();
            if (leaseLogCheck == null)
            {
                throw new Exception("无符合条件的借阅记录");
            }
            using IDatabaseTransactionManager transaction = database.StartTransaction();
            try
            {
                await database.ForUpdateAsync<BorrowLog, BorrowLog>(bl => bl.Id== leaseLogCheck .Id&& bl.User.Id == bl.User.Id && bl.Book.Id == bl.Book.Id, transaction: transaction.GetTransaction());
                leaseLog = (await database.SelectAsync<BorrowLog, BorrowLog>(bl => bl.Id == leaseLogCheck.Id && bl.User.Id == bl.User.Id && bl.Book.Id == bl.Book.Id, transaction: transaction.GetTransaction())).FirstOrDefault();
                if (leaseLog == null)
                {
                    throw new Exception("无符合条件的借阅记录");
                }

                leaseLog.GiveBack = DateTime.Now;
                int creditChange = credit.GetCreditChange(leaseLog);
                if (await database.UpdateAsync<Book, int>(b => b.Margin + 1, b => b.Id == leaseLog.Book.Id, transaction: transaction.GetTransaction()) == 0)
                {
                    throw new Exception("更新书本信息失败");
                }

                if (await database.UpdateAsync<User, int>(u => u.CreditValue + creditChange, u => u.Id == leaseLog.User.Id, transaction: transaction.GetTransaction()) == 0)
                {
                    throw new Exception("更新会员信息失败");
                }

                if (await database.UpdateAsync<BorrowLog, bool>(bl => bl.GiveBack == DateTime.Now, bl => bl.Id == leaseLog.Id, transaction: transaction.GetTransaction()) == 0)
                {
                    throw new Exception("更新借阅记录失败");
                }

                leaseLog = (await database.SelectAsync<BorrowLog, BorrowLog>(bl => bl.Id == leaseLog.Id&&bl.Book.Id== bl.Book.Id, transaction: transaction.GetTransaction())).FirstOrDefault();
                if (leaseLog == null)
                {
                    throw new Exception("更新借阅记录失败");
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            return leaseLog;
        }
        public async Task<List<BorrowLog>> QueryOverTimeLeaseAsync(User userId)
        {
            List<BorrowLog> leaseLogs = await database.SelectAsync<BorrowLog, BorrowLog>(bl => bl.Book.Id == bl.Book.Id && bl.UserId == userId.Id && bl.GiveBack == null);
            return leaseLogs;
        }
        public async Task<List<BorrowLog>> QueryAsync(Expression<Func<BorrowLog, bool>> whereExp = null, int pageIndex = 0, int pageSize = 0)
        {
            return await database.SelectAsync<BorrowLog, BorrowLog>(whereExp, pageIndex: pageIndex, pageSize: pageSize);
        }
        public async Task<long> CountAsync(Expression<Func<BorrowLog, bool>> whereExp = null)
        {
            return await database.CountAsync(whereExp);
        }
    }
}
