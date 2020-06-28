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
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>事务管理器</returns>
        public IDatabaseTransactionManager StartTransaction();
        /// <summary>
        /// 异步插入数据库操作
        /// </summary>
        /// <typeparam name="T">要插入的对象类型</typeparam>
        /// <param name="elements">要插入的对象集合</param>
        /// <param name="transaction">操作属于的事务</param>
        /// <returns>成功插入的行数</returns>
        public Task<int> InsertAsync<T>(
            IEnumerable<T> elements = null,
            DbTransaction transaction = null) where T : DatabaseModel;
        /// <summary>
        /// 异步更新数据库操作
        /// </summary>
        /// <typeparam name="T">要更新的对象类型</typeparam>
        /// <typeparam name="TMember">要更新的属性类型</typeparam>
        /// <param name="setExp"></param>
        /// <param name="whereExp"></param>
        /// <param name="ids"></param>
        /// <param name="transaction"></param>
        /// <returns>成功更新的行数</returns>
        public Task<int> UpdateAsync<T, TMember>(
            Expression<Func<T, TMember>> setExp,
            Expression<Func<T, bool>> whereExp = null,
            IEnumerable<T> ids = null,
            DbTransaction transaction = null) where T : DatabaseModel;
        /// <summary>
        /// 异步查询操作
        /// </summary>
        /// <typeparam name="T">要查询的对象类型</typeparam>
        /// <typeparam name="TNavigate">联合查询对象类型</typeparam>
        /// <param name="whereExp">查询条件表达式</param>
        /// <param name="ids">对象主键</param>
        /// <param name="navigateExp">联合查询表达式</param>
        /// <param name="pageIndex">分页查询页号</param>
        /// <param name="pageSize">分页查询每页大小</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
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
