﻿namespace System.Net.Sockets
{
    using System;

    public class LingerOption
    {
        private bool enabled;
        private int lingerTime;

        public LingerOption(bool enable, int seconds)
        {
            this.Enabled = enable;
            this.LingerTime = seconds;
        }

        public bool Enabled
        {
            get => 
                this.enabled;
            set
            {
                this.enabled = value;
            }
        }

        public int LingerTime
        {
            get => 
                this.lingerTime;
            set
            {
                this.lingerTime = value;
            }
        }
    }
}

