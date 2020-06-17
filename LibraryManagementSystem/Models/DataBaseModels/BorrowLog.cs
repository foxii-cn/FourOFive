using FreeSql.DataAnnotations;
using System;

namespace LibraryManagementSystem.Models.DataBaseModels
{
    [Table()]
    public class BorrowLog : DatabaseModel
    {
        [Column(IsPrimary = true)]
        public Guid Id { get; set; }
        [Column()]
        public Guid BookId { set; get; }
        [Column()]
        public virtual Book Book { set; get; }
        [Column()]
        public Guid UserId { set; get; }
        [Column()]
        public virtual User User { set; get; }
        [Column()]
        public int CreditValueHistory { get; set; }
        [Column(IsNullable = false)]
        public DateTime? Deadline { set; get; }
        [Column()]
        public DateTime? GiveBack { set; get; }

    }
}
