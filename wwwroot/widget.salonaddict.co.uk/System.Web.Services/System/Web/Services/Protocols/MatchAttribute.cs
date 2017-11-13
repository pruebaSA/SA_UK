namespace System.Web.Services.Protocols
{
    using System;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class MatchAttribute : Attribute
    {
        private int capture;
        private int group = 1;
        private bool ignoreCase;
        private string pattern;
        private int repeats = -1;

        public MatchAttribute(string pattern)
        {
            this.pattern = pattern;
        }

        public int Capture
        {
            get => 
                this.capture;
            set
            {
                this.capture = value;
            }
        }

        public int Group
        {
            get => 
                this.group;
            set
            {
                this.group = value;
            }
        }

        public bool IgnoreCase
        {
            get => 
                this.ignoreCase;
            set
            {
                this.ignoreCase = value;
            }
        }

        public int MaxRepeats
        {
            get => 
                this.repeats;
            set
            {
                this.repeats = value;
            }
        }

        public string Pattern
        {
            get
            {
                if (this.pattern != null)
                {
                    return this.pattern;
                }
                return string.Empty;
            }
            set
            {
                this.pattern = value;
            }
        }
    }
}

