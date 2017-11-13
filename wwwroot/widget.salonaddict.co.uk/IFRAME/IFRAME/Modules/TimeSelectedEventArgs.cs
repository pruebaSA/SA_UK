namespace IFRAME.Modules
{
    using System;
    using System.Runtime.CompilerServices;

    public class TimeSelectedEventArgs : EventArgs
    {
        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }
    }
}

