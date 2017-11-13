namespace System.Windows.Forms
{
    using System;

    public class CacheVirtualItemsEventArgs : EventArgs
    {
        private int endIndex;
        private int startIndex;

        public CacheVirtualItemsEventArgs(int startIndex, int endIndex)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        public int EndIndex =>
            this.endIndex;

        public int StartIndex =>
            this.startIndex;
    }
}

