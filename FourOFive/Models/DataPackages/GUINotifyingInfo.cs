using System;

namespace FourOFive.Models.DataPackages
{
    public class GUINotifyingInfo
    {
        public NotifyingType Type { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public string Message { get; set; }
    }
}
