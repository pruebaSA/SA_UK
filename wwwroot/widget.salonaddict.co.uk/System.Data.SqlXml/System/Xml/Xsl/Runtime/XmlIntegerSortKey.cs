namespace System.Xml.Xsl.Runtime
{
    using System;

    internal class XmlIntegerSortKey : XmlSortKey
    {
        private long longVal;

        public XmlIntegerSortKey(long value, XmlCollation collation)
        {
            this.longVal = collation.DescendingOrder ? ~value : value;
        }

        public override int CompareTo(object obj)
        {
            XmlIntegerSortKey that = obj as XmlIntegerSortKey;
            if (that == null)
            {
                return base.CompareToEmpty(obj);
            }
            if (this.longVal == that.longVal)
            {
                return base.BreakSortingTie(that);
            }
            if (this.longVal >= that.longVal)
            {
                return 1;
            }
            return -1;
        }
    }
}

