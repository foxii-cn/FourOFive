﻿using FreeSql.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    [Table()]
    public class ISBNInfo : DatabaseModel
    {
        [Column(IsPrimary = true)]
        public string ISBN { get; set; }
        [Column(IsNullable = false)]
        public string Title { get; set; }
        [Column()]
        public string Author { get; set; }
        [Column()]
        public string PublishingHouse { get; set; }
        [Column()]
        public string Labels { get; set; }
        [Column()]
        public string CoverUrl { get; set; }
    }
}
