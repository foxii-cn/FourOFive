using FreeSql.DataAnnotations;

namespace LibraryManagementSystem.Entity
{
    [Table()]
    [Index("uk_NationalIdentificationNumber", "NationalIdentificationNumber", true)]
    [Index("uk_UserName", "UserName", true)]
    public class User : BasicEntity
    {
        [Column(IsNullable = false)]
        public string UserName { get; set; }
        [Column(IsNullable = false)]
        public string Salt { get; set; }
        [Column(IsNullable = false)]
        public string Password { get; set; }
        [Column(IsNullable = false)]
        public string Name { get; set; }
        [Column(IsNullable = false)]
        public string NationalIdentificationNumber { get; set; }
        [Column()]
        public int CreditValue { get; set; }

    }
}
