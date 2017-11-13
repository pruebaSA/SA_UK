namespace System.Xml.Xsl.Runtime
{
    using System;

    internal class XmlDecimalSortKey : XmlSortKey
    {
        private decimal decVal;

        public XmlDecimalSortKey(decimal value, XmlCollation collation)
        {
            this.decVal = collation.DescendingOrder ? -value : value;
        }

        public override int CompareTo(object obj)
        {
            XmlDecimalSortKey that = obj as XmlDecimalSortKey;
            if (that == null)
            {
                return base.CompareToEmpty(obj);
            }
            int num = decimal.Compare(this.decVal, that.decVal);
            if (num == 0)
            {
                return base.BreakSortingTie(that);
            }
            return num;
        }
    }
}

