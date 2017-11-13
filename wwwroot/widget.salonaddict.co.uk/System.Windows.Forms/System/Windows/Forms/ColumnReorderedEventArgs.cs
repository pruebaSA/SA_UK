namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    public class ColumnReorderedEventArgs : CancelEventArgs
    {
        private ColumnHeader header;
        private int newDisplayIndex;
        private int oldDisplayIndex;

        public ColumnReorderedEventArgs(int oldDisplayIndex, int newDisplayIndex, ColumnHeader header)
        {
            this.oldDisplayIndex = oldDisplayIndex;
            this.newDisplayIndex = newDisplayIndex;
            this.header = header;
        }

        public ColumnHeader Header =>
            this.header;

        public int NewDisplayIndex =>
            this.newDisplayIndex;

        public int OldDisplayIndex =>
            this.oldDisplayIndex;
    }
}

