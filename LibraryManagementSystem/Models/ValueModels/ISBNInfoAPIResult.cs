using System;

namespace LibraryManagementSystem.Models.ValueModels
{
    public class ISBNInfoAPIResult
    {
        public ISBNInfo ISBNInfo { get; set; }
        public Exception Exception { get; set; }
    }
}
