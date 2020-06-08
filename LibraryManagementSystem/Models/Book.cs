using FreeSql.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    [Table()]
    public class Book : DatabaseModel
    {
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
