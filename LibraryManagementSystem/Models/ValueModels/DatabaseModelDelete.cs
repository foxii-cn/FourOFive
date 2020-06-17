using System;
using System.Data.Common;

namespace LibraryManagementSystem.Models.ValueModels
{
    public class DatabaseModelDelete
    {
        public object DYWhere { get; set; } = null;

        public DbTransaction Transaction { get; set; } = null;

        public int AffectedRows { get; set; } = 0;

        public Exception Exception { get; set; } = null;
    }
}
