namespace System.IO.Compression
{
    using System;

    internal class DeflateInput
    {
        private byte[] buffer;
        private int count;
        private int startIndex;

        internal void ConsumeBytes(int n)
        {
            this.startIndex += n;
            this.count -= n;
        }

        internal byte[] Buffer
        {
            get => 
                this.buffer;
            set
            {
                this.buffer = value;
            }
        }

        internal int Count
        {
            get => 
                this.count;
            set
            {
                this.count = value;
            }
        }

        internal int StartIndex
        {
            get => 
                this.startIndex;
            set
            {
                this.startIndex = value;
            }
        }
    }
}

