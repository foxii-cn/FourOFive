using FreeSql;
using System.Data.Common;

namespace FourOFive.Managers.Implements
{
    public class FreeSQLDatabaseTransactionManager : IDatabaseTransactionManager
    {
        public IUnitOfWork unitOfWork;
        public FreeSQLDatabaseTransactionManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public void Commit()
        {
            unitOfWork.Commit();
        }

        public void Dispose()
        {
            unitOfWork.Dispose();
        }

        public DbTransaction GetTransaction()
        {
            return unitOfWork.GetOrBeginTransaction();
        }

        public void Rollback()
        {
            unitOfWork.Rollback();
        }
    }
}
