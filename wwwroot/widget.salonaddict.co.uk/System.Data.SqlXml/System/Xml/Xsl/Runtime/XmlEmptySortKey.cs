namespace System.Xml.Xsl.Runtime
{
    using System;

    internal class XmlEmptySortKey : XmlSortKey
    {
        private bool isEmptyGreatest;

        public XmlEmptySortKey(XmlCollation collation)
        {
            this.isEmptyGreatest = collation.EmptyGreatest != collation.DescendingOrder;
        }

        public override int CompareTo(object obj)
        {
            XmlEmptySortKey that = obj as XmlEmptySortKey;
            if (that == null)
            {
                return -(obj as XmlSortKey).CompareTo(this);
            }
            return base.BreakSortingTie(that);
        }

        public bool IsEmptyGreatest =>
            this.isEmptyGreatest;
    }
}

