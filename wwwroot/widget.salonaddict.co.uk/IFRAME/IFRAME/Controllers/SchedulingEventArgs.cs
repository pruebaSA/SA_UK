namespace IFRAME.Controllers
{
    using System;
    using System.Runtime.CompilerServices;

    public class SchedulingEventArgs : EventArgs
    {
        public TimeSlotUI Time { get; set; }
    }
}

