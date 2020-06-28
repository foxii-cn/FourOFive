using System;

namespace FourOFive.Models.DataPackages
{
    public class GUINotifyingDataPackage
    {
        public NotifyingType Type { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public string Message { get; set; }
    }
}
