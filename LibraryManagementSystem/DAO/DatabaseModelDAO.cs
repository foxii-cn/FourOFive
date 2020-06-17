using FreeSql;
using LibraryManagementSystem.Models.DataBaseModels;
using LibraryManagementSystem.Models.ValueModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;

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
        public IObservable<DatabaseModelCreate<T>> Create(IObservable<DatabaseModelCreate<T>> createStream) =>
            createStream.Select(createInfo =>
            {
                try
                {
                    createInfo.AffectedRows = sql
                    .Insert(createInfo.DatabaseModels)
                    .WithTransaction(createInfo.Transaction)
                    .ExecuteAffrows();
                }
                catch (Exception ex)
                {
                    createInfo.Exception = ex;
                }
                return createInfo;
            });
        public int Create(params T[] elements)
        {
            return sql.Insert(elements)
                .ExecuteAffrows();
        }
        public IObservable<DatabaseModelDelete> Delete(IObservable<DatabaseModelDelete> deleteStream) =>
            deleteStream.Select(deleteInfo =>
            {
                try
                {
                    deleteInfo.AffectedRows = sql
                    .Delete<T>(deleteInfo.DYWhere)
                    .WithTransaction(deleteInfo.Transaction)
                    .ExecuteAffrows();
                }
                catch (Exception ex)
                {
                    deleteInfo.Exception = ex;
                }
                return deleteInfo;
            });
        public int Delete(object dywhere)
        {
            return sql.Delete<T>(dywhere)
                .ExecuteAffrows();
        }
        public IObservable<DatabaseModelUpdate> Update(IObservable<DatabaseModelUpdate> updateStream) =>
            updateStream.Select(updateInfo =>
            {
                try
                {
                    updateInfo.AffectedRows = sql
                    .Update<T>(updateInfo.DYWhere)
                    .SetRaw(updateInfo.SetSql, updateInfo.Parms)
                    .WithTransaction(updateInfo.Transaction)
                    .ExecuteAffrows();
                }
                catch (Exception ex)
                {
                    updateInfo.Exception = ex;
                }
                return updateInfo;
            });
        public int Update(object dywhere, string setSql, object parms)
        {
            if (sql.Ado.TransactionCurrentThread == null)
                throw new Exception("{LogName}: 未在事务中时更新对象");
            return sql.Update<T>(dywhere)
            .SetRaw(setSql, parms)
            .ExecuteAffrows();
        }
        public IObservable<DatabaseModelQuery<T, TNavigate>> Query<TNavigate>(IObservable<DatabaseModelQuery<T, TNavigate>> queryStream) where TNavigate : DatabaseModel =>
            queryStream.Select(queryInfo =>
            {
                try
                {
                    ISelect<T> select = sql
                    .Select<T>(queryInfo.DYWhere)
                    .Where(queryInfo.ConditionSql, queryInfo.Parms)
                    .Where(queryInfo.ConditionExpression)
                    .Include(queryInfo.IncludeExpression)
                    .Count(out long count)
                    .WithTransaction(queryInfo.Transaction);
                    if (queryInfo.PageSize > 0)
                        select = select.Page(queryInfo.PageIndex, queryInfo.PageSize);
                    queryInfo.DatabaseModels = select.ToList();
                    queryInfo.Count = count;
                }
                catch (Exception ex)
                {
                    queryInfo.Exception = ex;
                }
                return queryInfo;
            });
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
