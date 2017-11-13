namespace System.Windows.Forms
{
    using System;
    using System.Drawing;

    public class SearchForVirtualItemEventArgs : EventArgs
    {
        private SearchDirectionHint direction;
        private bool includeSubItemsInSearch;
        private int index = -1;
        private bool isPrefixSearch;
        private bool isTextSearch;
        private int startIndex;
        private Point startingPoint;
        private string text;

        public SearchForVirtualItemEventArgs(bool isTextSearch, bool isPrefixSearch, bool includeSubItemsInSearch, string text, Point startingPoint, SearchDirectionHint direction, int startIndex)
        {
            this.isTextSearch = isTextSearch;
            this.isPrefixSearch = isPrefixSearch;
            this.includeSubItemsInSearch = includeSubItemsInSearch;
            this.text = text;
            this.startingPoint = startingPoint;
            this.direction = direction;
            this.startIndex = startIndex;
        }

        public SearchDirectionHint Direction =>
            this.direction;

        public bool IncludeSubItemsInSearch =>
            this.includeSubItemsInSearch;

        public int Index
        {
            get => 
                this.index;
            set
            {
                this.index = value;
            }
        }

        public bool IsPrefixSearch =>
            this.isPrefixSearch;

        public bool IsTextSearch =>
            this.isTextSearch;

        public int StartIndex =>
            this.startIndex;

        public Point StartingPoint =>
            this.startingPoint;

        public string Text =>
            this.text;
    }
}

