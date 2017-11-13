namespace System.Xml.Xsl.Runtime
{
    using System;

    internal abstract class XmlSortKey : IComparable
    {
        private XmlSortKey nextKey;
        private int priority;

        protected XmlSortKey()
        {
        }

        public XmlSortKey AddSortKey(XmlSortKey sortKey)
        {
            if (this.nextKey != null)
            {
                this.nextKey.AddSortKey(sortKey);
            }
            else
            {
                this.nextKey = sortKey;
            }
            return this;
        }

        protected int BreakSortingTie(XmlSortKey that)
        {
            if (this.nextKey != null)
            {
                return this.nextKey.CompareTo(that.nextKey);
            }
            if (this.priority >= that.priority)
            {
                return 1;
            }
            return -1;
        }

        public abstract int CompareTo(object that);
        protected int CompareToEmpty(object obj)
        {
            XmlEmptySortKey key = obj as XmlEmptySortKey;
            if (!key.IsEmptyGreatest)
            {
                return 1;
            }
            return -1;
        }

        public int Priority
        {
            set
            {
                for (XmlSortKey key = this; key != null; key = key.nextKey)
                {
                    key.priority = value;
                }
            }
        }
    }
}

