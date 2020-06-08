using FreeSql.DataAnnotations;
using System;

namespace LibraryManagementSystem.Models
{
    [Table()]
    public class BorrowLog : DatabaseModel
    {
        [Column(IsPrimary = true)]
        public Guid BookId { set; get; }
        [Column()]
        public virtual Book Book { set; get; }
        [Column(IsPrimary = true)]
        public Guid UserId { set; get; }
        [Column()]
        public virtual User User { set; get; }
        [Column(IsNullable = false)]
        public DateTime? Deadline { set; get; }
        [Column()]
        public DateTime? GiveBack { set; get; }

    }
}
