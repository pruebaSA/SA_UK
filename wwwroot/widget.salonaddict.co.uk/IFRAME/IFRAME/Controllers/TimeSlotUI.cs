namespace IFRAME.Controllers
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class TimeSlotUI
    {
        public int Slots { get; set; }

        public TimeSpan Time { get; set; }

        public DayOfWeek WeekDay { get; set; }
    }
}

