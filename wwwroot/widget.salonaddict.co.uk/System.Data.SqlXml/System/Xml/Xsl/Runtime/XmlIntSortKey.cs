namespace System.Xml.Xsl.Runtime
{
    using System;

    internal class XmlIntSortKey : XmlSortKey
    {
        private int intVal;

        public XmlIntSortKey(int value, XmlCollation collation)
        {
            this.intVal = collation.DescendingOrder ? ~value : value;
        }

        public override int CompareTo(object obj)
        {
            XmlIntSortKey that = obj as XmlIntSortKey;
            if (that == null)
            {
                return base.CompareToEmpty(obj);
            }
            if (this.intVal == that.intVal)
            {
                return base.BreakSortingTie(that);
            }
            if (this.intVal >= that.intVal)
            {
                return 1;
            }
            return -1;
        }
    }
}

