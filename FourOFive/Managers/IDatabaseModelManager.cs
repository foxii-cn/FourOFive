using FourOFive.Models.DataBaseModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FourOFive.Managers
{
    public interface IDatabaseModelManager
    {
        public IDatabaseTransactionManager StartTransaction();

        public Task<int> InsertAsync<T>(
            IEnumerable<T> elements = null,
            DbTransaction transaction = null) where T : DatabaseModel;

        public Task<int> UpdateAsync<T, TMember>(
            Expression<Func<T, TMember>> setExp,
            Expression<Func<T, bool>> whereExp = null,
            IEnumerable<T> ids = null,
            DbTransaction transaction = null) where T : DatabaseModel;

        public Task<List<T>> SelectAsync<T, TNavigate>(
            Expression<Func<T, bool>> whereExp = null,
            IEnumerable<T> ids = null,
            Expression<Func<T, TNavigate>> navigateExp = null,
            int pageIndex = 0,
            int pageSize = 0,
            DbTransaction transaction = null) where T : DatabaseModel where TNavigate : DatabaseModel;

        public Task<long> CountAsync<T>(
            Expression<Func<T, bool>> whereExp = null,
            IEnumerable<T> ids = null,
            DbTransaction transaction = null) where T : DatabaseModel;

        public Task<int> DeleteAsync<T, TNavigate>(
            Expression<Func<T, bool>> whereExp = null,
            IEnumerable<T> ids = null,
            Expression<Func<T, TNavigate>> navigateExp = null,
            DbTransaction transaction = null) where T : DatabaseModel where TNavigate : DatabaseModel;

        public Task ForUpdateAsync<T, TNavigate>(
            Expression<Func<T, bool>> whereExp = null,
            IEnumerable<T> ids = null,
            Expression<Func<T, TNavigate>> navigateExp = null,
            DbTransaction transaction = null) where T : DatabaseModel where TNavigate : DatabaseModel;
    }
}
