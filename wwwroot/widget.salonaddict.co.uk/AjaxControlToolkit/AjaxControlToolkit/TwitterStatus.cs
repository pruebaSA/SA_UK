namespace AjaxControlToolkit
{
    using System;
    using System.Runtime.CompilerServices;

    public class TwitterStatus
    {
        public DateTime CreatedAt { get; set; }

        public string Text { get; set; }

        public TwitterUser User { get; set; }
    }
}

