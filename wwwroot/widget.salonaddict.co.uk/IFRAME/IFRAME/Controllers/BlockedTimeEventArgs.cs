namespace IFRAME.Controllers
{
    using System;
    using System.Runtime.CompilerServices;

    public class BlockedTimeEventArgs : EventArgs
    {
        public Guid BlockedTimeId { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }
    }
}

