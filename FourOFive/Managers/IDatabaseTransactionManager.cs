using System;
using System.Data.Common;

namespace FourOFive.Managers
{
    public interface IDatabaseTransactionManager : IDisposable
    {
        public void Commit();
        public void Rollback();
        public DbTransaction GetTransaction();
    }
}
