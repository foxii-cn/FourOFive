using FreeSql.DataAnnotations;
using System;

namespace LibraryManagementSystem.Entity
{
    [Table()]
    public class LeaseLog : BasicEntity
    {
        [Column()]
        public Guid BookId { set; get; }
        [Column()]
        public Guid MemberId { set; get; }
        [Column(IsNullable = false)]
        public DateTime? Deadline { set; get; }
        [Column()]
        public DateTime? GiveBack { set; get; }

    }
}
