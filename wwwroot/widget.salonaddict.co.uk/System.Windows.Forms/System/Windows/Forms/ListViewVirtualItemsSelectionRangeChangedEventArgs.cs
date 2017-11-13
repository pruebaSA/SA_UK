﻿namespace System.Windows.Forms
{
    using System;

    public class ListViewVirtualItemsSelectionRangeChangedEventArgs : EventArgs
    {
        private int endIndex;
        private bool isSelected;
        private int startIndex;

        public ListViewVirtualItemsSelectionRangeChangedEventArgs(int startIndex, int endIndex, bool isSelected)
        {
            if (startIndex > endIndex)
            {
                throw new ArgumentException(System.Windows.Forms.SR.GetString("ListViewStartIndexCannotBeLargerThanEndIndex"));
            }
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.isSelected = isSelected;
        }

        public int EndIndex =>
            this.endIndex;

        public bool IsSelected =>
            this.isSelected;

        public int StartIndex =>
            this.startIndex;
    }
}

