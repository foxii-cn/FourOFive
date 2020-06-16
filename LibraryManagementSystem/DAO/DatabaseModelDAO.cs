using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LibraryManagementSystem.DAO
{
    public class DatabaseModelDAO<T> where T : DatabaseModel
    {
        // 数据库对象
        private readonly IFreeSql sql;


        public DatabaseModelDAO(IFreeSql sql)
        {
            this.sql = sql;
        }
        public int Create(params T[] elements)
        {
            return sql.Insert(elements)
                .ExecuteAffrows();
        }
        public int Delete(object dywhere)
        {
            return sql.Delete<T>(dywhere)
                .ExecuteAffrows();
        }
        public int Update(object dywhere, string setSql, object parms)
        {
            if (sql.Ado.TransactionCurrentThread == null)
                throw new Exception("{LogName}: 未在事务中时更新对象");
            return sql.Update<T>(dywhere)
            .SetRaw(setSql, parms)
            .ExecuteAffrows();
        }
        public List<T> QuerySql(string condition, int pageIndex, int pageSize, out long count, object parms)
        {
            return sql.Select<T>()
            .Where(condition, parms)
            .Count(out count)
            .Page(pageIndex, pageSize)
            .ToList();
        }
        public List<T> QuerySql(string condition, object parms)
        {
            return sql.Select<T>()
            .Where(condition, parms)
            .ToList();
        }
        public List<T> Query(object dywhere)
        {
            return sql.Select<T>(dywhere)
                .ToList();
        }
        public List<T> QueryLambda<TNavigate>(Expression<Func<T, bool>> whereExp, Expression<Func<T, TNavigate>> includeExp = null) where TNavigate : DatabaseModel
        {
            return sql.Select<T>()
                .Where(whereExp)
                .Include(includeExp)
                .ToList();
        }
        public List<T> QueryLambda<TNavigate>(Expression<Func<T, bool>> whereExp, int pageIndex, int pageSize, out long count, Expression<Func<T, TNavigate>> includeExp = null) where TNavigate : DatabaseModel
        {
            return sql.Select<T>()
                .Where(whereExp)
                .Include(includeExp)
                .Count(out count)
                .Page(pageIndex, pageSize)
                .ToList();
        }
        public void ForUpdate(object dywhere)
        {
            sql.Select<T>(dywhere)
                .ForUpdate()
                .ToOne();
        }
        public void ForUpdateSql(string condition, object parms)
        {
            sql.Select<T>()
            .Where(condition, parms)
            .ForUpdate()
            .ToOne();
        }
        public void Transaction(Action handler)
        {
            sql.Transaction(handler);
        }
    }
}
