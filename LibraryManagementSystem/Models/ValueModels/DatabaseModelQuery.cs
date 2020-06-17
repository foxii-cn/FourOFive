using LibraryManagementSystem.Models.DataBaseModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;

namespace LibraryManagementSystem.Models.ValueModels
{
    public class DatabaseModelQuery<T, TNavigate> where T : DatabaseModel where TNavigate : DatabaseModel
    {
        public object DYWhere { get; set; } = null;

        public string ConditionSql { get; set; } = null;
        public object Parms { get; set; } = null;

        public Expression<Func<T, bool>> ConditionExpression { get; set; } = null;

        public Expression<Func<T, TNavigate>> IncludeExpression { get; set; } = null;

        public int PageSize { get; set; } = 0;
        public int PageIndex { get; set; } = 0;

        public DbTransaction Transaction { get; set; } = null;

        public List<T> DatabaseModels { get; set; } = null;
        public long Count { get; set; } = 0;

        public Exception Exception { get; set; } = null;
    }
}
