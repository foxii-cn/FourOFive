using LibraryManagementSystem.Models.DataBaseModels;
using System;

namespace LibraryManagementSystem.Models.ValueModels
{
    public class ISBNInfoGet
    {
        public string ISBN { get; set; } = null;

        public ISBNInfo ISBNInfo { get; set; } = null;

        public Exception Exception { get; set; } = null;
    }
}
