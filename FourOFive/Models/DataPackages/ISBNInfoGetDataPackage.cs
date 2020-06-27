using FourOFive.Models.DataBaseModels;
using System;
using System.Collections.Generic;

namespace FourOFive.Models.DataPackages
{
    public class ISBNInfoGetDataPackage
    {
        public string ISBN { get; set; } = null;

        public ISBNInfo ISBNInfo { get; set; } = null;

        public List<Exception> Exceptions { get; set; } = new List<Exception>();
    }
}
