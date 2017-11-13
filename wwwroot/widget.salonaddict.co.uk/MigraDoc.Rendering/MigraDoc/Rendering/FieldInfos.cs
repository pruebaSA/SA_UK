namespace MigraDoc.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class FieldInfos
    {
        private Dictionary<string, BookmarkInfo> bookmarks;
        internal DateTime date;
        internal int displayPageNr;
        internal int numPages;
        internal int pyhsicalPageNr;
        internal int section;
        internal int sectionPages;

        internal FieldInfos(Dictionary<string, BookmarkInfo> bookmarks)
        {
            this.bookmarks = bookmarks;
        }

        internal void AddBookmark(string name)
        {
            if (this.pyhsicalPageNr > 0)
            {
                if (this.bookmarks.ContainsKey(name))
                {
                    this.bookmarks.Remove(name);
                }
                if (this.pyhsicalPageNr > 0)
                {
                    this.bookmarks.Add(name, new BookmarkInfo(this.pyhsicalPageNr, this.displayPageNr));
                }
            }
        }

        internal int GetPhysicalPageNumber(string bookmarkName)
        {
            if (this.bookmarks.ContainsKey(bookmarkName))
            {
                BookmarkInfo info = this.bookmarks[bookmarkName];
                return info.displayPageNumber;
            }
            return -1;
        }

        internal int GetShownPageNumber(string bookmarkName)
        {
            if (this.bookmarks.ContainsKey(bookmarkName))
            {
                BookmarkInfo info = this.bookmarks[bookmarkName];
                return info.shownPageNumber;
            }
            return -1;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct BookmarkInfo
        {
            internal int displayPageNumber;
            internal int shownPageNumber;
            internal BookmarkInfo(int physicalPageNumber, int displayPageNumber)
            {
                this.displayPageNumber = physicalPageNumber;
                this.shownPageNumber = displayPageNumber;
            }
        }
    }
}

