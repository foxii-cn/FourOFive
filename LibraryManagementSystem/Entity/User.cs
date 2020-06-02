using FreeSql.DataAnnotations;
using System;

namespace LibraryManagementSystem.Entity
{
    [Index("uk_NationalIdentificationNumber", "NationalIdentificationNumber", true)]
    [Index("uk_UserName", "UserName", true)]
    public abstract class User : BasicEntity
    {
        [Column(IsNullable = false)]
        public string UserName { get; set; }
        [Column(IsNullable = false)]
        public string Password { get; set; }
        [Column(IsNullable = false)]
        public string Name { get; set; }
        [Column(IsNullable = false)]
        public string NationalIdentificationNumber { get; set; }
    }
}
