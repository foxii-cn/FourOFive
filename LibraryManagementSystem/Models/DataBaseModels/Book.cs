using FreeSql.DataAnnotations;
using System;

namespace LibraryManagementSystem.Models.DataBaseModels
{
    [Table()]
    public class Book : DatabaseModel
    {
        [Column(IsPrimary = true)]
        public Guid Id { get; set; }
        [Column(IsNullable = false)]
        public string Title { get; set; }
        [Column()]
        public string Author { get; set; }
        [Column()]
        public string PublishingHouse { get; set; }
        [Column()]
        public int Margin { set; get; } = 0;
        [Column()]
        public int Sum { set; get; } = 0;
        [Column()]
        public string Position { set; get; }
    }
}
