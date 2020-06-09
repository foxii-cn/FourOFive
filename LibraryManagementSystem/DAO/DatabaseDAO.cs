using LibraryManagementSystem.Models;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LibraryManagementSystem.DAO
{

    public abstract class DatabaseDAO<T> where T : DatabaseModel
    {
        // 数据库对象
        private readonly IFreeSql sql;
        // 日志记录对象
        private readonly Logger logger;
        // 日志主体
        private readonly string LogName;


        public DatabaseDAO(IFreeSql sql, Logger logger)
        {
            LogName = GetType().Name;
            this.sql = sql;
            this.logger = logger;
        }
        public int Create(params T[] elements)
        {
            FreeSql.IInsert<T> insert;
            int rows;
            try
            {
                insert = sql.Insert(elements);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 为{Elements}创建插入对象失败",
                                    LogName, elements);
                throw;
            }
            try
            {
                rows = insert.ExecuteAffrows();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行查询操作{InsertSql}失败",
                                    LogName, insert.ToSql());
                throw;
            }
            return rows;
        }
        public int Delete(object dywhere)
        {
            FreeSql.IDelete<T> delete;
            int rows;
            try
            {
                delete = sql.Delete<T>(dywhere);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 以{DYWhere}创建删除对象失败",
                                    LogName, dywhere);
                throw;
            }
            try
            {
                rows = delete.ExecuteAffrows();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行删除操作{DeleteSql}失败",
                                    LogName, delete.ToSql());
                throw;
            }
            return rows;
        }
        public int Update(object dywhere, string setSql, object parms)
        {
            if (sql.Ado.TransactionCurrentThread == null)
                logger.Warning("{LogName}: 未在事务中时更新对象",
                                    LogName);
            FreeSql.IUpdate<T> update;
            int rows;
            try
            {
                update = sql.Update<T>(dywhere)
                .SetRaw(setSql, parms);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 以{DYWhere}创建{SetSql}更新对象失败",
                                    LogName, dywhere, setSql);
                throw;
            }
            try
            {
                rows = update.ExecuteAffrows();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行更新操作{UpdateSql}失败",
                                    LogName, update.ToSql());
                throw;
            }
            return rows;
        }
        public List<T> QuerySql(string condition, int pageIndex, int pageSize, out long count, object parms)
        {
            FreeSql.ISelect<T> select;
            List<T> elements;
            try
            {
                select = sql.Select<T>()
                .Where(condition, parms)
                .Count(out count)
                .Page(pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 为类型{T}以条件{Condition}创建查询对象失败",
                                    LogName, typeof(T), condition);
                throw;
            }
            try
            {
                elements = select.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行查询操作{SelectSql}失败",
                                    LogName, select.ToSql());
                throw;
            }
            return elements;
        }
        public List<T> QuerySql(string condition, object parms)
        {
            FreeSql.ISelect<T> select;
            List<T> elements;
            try
            {
                select = sql.Select<T>()
                .Where(condition, parms);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 为类型{T}以条件{Condition}创建查询对象失败",
                                    LogName, typeof(T), condition);
                throw;
            }
            try
            {
                elements = select.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行查询操作{SelectSql}失败",
                                    LogName, select.ToSql());
                throw;
            }
            return elements;
        }
        public List<T> Query(object dywhere)
        {
            FreeSql.ISelect<T> select;
            List<T> refreshedElements;
            try
            {
                select = sql.Select<T>(dywhere);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 为类型{T}以{DYWhere}创建查询对象失败",
                                    LogName, typeof(T), dywhere);
                throw;
            }
            try
            {
                refreshedElements = select.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行查询操作{SelectSql}失败",
                                    LogName, select.ToSql());
                throw;
            }
            return refreshedElements;
        }
        public List<T> QueryLambda<TNavigate>(Expression<Func<T, bool>> whereExp, Expression<Func<T, TNavigate>> includeExp = null) where TNavigate : DatabaseModel
        {
            FreeSql.ISelect<T> select;
            List<T> selectedElements;
            try
            {
                select = sql.Select<T>()
                    .Where(whereExp)
                    .Include(includeExp);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 为类型{T}以{Exp}创建查询对象失败",
                                    LogName, typeof(T), whereExp);
                throw;
            }
            try
            {
                selectedElements = select.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行查询操作{SelectSql}失败",
                                    LogName, select.ToSql());
                throw;
            }
            return selectedElements;
        }
            public List<T> QueryLambda<TNavigate>(Expression<Func<T, bool>> whereExp, int pageIndex, int pageSize, out long count, Expression<Func<T, TNavigate>> includeExp = null) where TNavigate : DatabaseModel
        {
            FreeSql.ISelect<T> select;
            List<T> selectedElements;
            try
            {
                select = sql.Select<T>()
                    .Where(whereExp)
                    .Include(includeExp)
                    .Count(out count)
                    .Page(pageIndex,pageSize);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 为类型{T}以{Exp}创建查询对象失败",
                                    LogName, typeof(T), whereExp);
                throw;
            }
            try
            {
                selectedElements = select.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行查询操作{SelectSql}失败",
                                    LogName, select.ToSql());
                throw;
            }
            return selectedElements;
        }
        public void ForUpdate(object dywhere)
        {
            FreeSql.ISelect<T> forUpdate;
            try
            {
                forUpdate = sql.Select<T>(dywhere)
                    .ForUpdate();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 为类型{T}以{DYWhere}创建悲观锁对象失败",
                                    LogName, typeof(T), dywhere);
                throw;
            }
            try
            {
                forUpdate.ToOne();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行查询操作{ForUpdateSql}失败",
                                    LogName, forUpdate.ToSql());
                throw;
            }
        }
        public void ForUpdateSql(string condition, object parms)
        {
            FreeSql.ISelect<T> forUpdate;
            try
            {
                forUpdate = sql.Select<T>()
                .Where(condition, parms)
                .ForUpdate();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 为类型{T}以条件{Condition}创建悲观锁对象失败",
                                    LogName, typeof(T), condition);
                throw;
            }
            try
            {
                forUpdate.ToOne();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "{LogName}: 执行查询操作{ForUpdateSql}失败",
                                    LogName, forUpdate.ToSql());
                throw;
            }
        }
        public void Transaction(Action handler)
        {
            sql.Transaction(handler);
        }

    }

}
