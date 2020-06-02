using FreeSql.DataAnnotations;

namespace LibraryManagementSystem.Entity
{
    [Table()]
    public class Member : User
    {
        [Column()]
        public int CreditValue { get; set; }

    }
}
