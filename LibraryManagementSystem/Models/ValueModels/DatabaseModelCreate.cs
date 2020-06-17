using LibraryManagementSystem.Models.DataBaseModels;
using System;
using System.Data.Common;

namespace LibraryManagementSystem.Models.ValueModels
{
    public class DatabaseModelCreate<T> where T : DatabaseModel
    {
        public T[] DatabaseModels { get; set; } = null;

        public DbTransaction Transaction { get; set; } = null;

        public int AffectedRows { get; set; } = 0;

        public Exception Exception { get; set; } = null;
    }
}
