namespace System.IO.Compression
{
    using System;

    internal class Match
    {
        private int len;
        private int pos;
        private MatchState state;
        private byte symbol;

        internal int Length
        {
            get => 
                this.len;
            set
            {
                this.len = value;
            }
        }

        internal int Position
        {
            get => 
                this.pos;
            set
            {
                this.pos = value;
            }
        }

        internal MatchState State
        {
            get => 
                this.state;
            set
            {
                this.state = value;
            }
        }

        internal byte Symbol
        {
            get => 
                this.symbol;
            set
            {
                this.symbol = value;
            }
        }
    }
}

