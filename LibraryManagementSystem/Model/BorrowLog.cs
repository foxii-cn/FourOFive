using FreeSql.DataAnnotations;
using System;

namespace LibraryManagementSystem.Model
{
    [Table()]
    public class BorrowLog : DatabaseModel
    {
        [Column()]
        public Guid BookId { set; get; }
        [Column()]
        public Guid UserId { set; get; }
        [Column(IsNullable = false)]
        public DateTime? Deadline { set; get; }
        [Column()]
        public DateTime? GiveBack { set; get; }

    }
}
