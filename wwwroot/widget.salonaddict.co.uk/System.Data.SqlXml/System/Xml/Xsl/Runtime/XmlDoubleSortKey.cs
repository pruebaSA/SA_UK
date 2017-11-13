namespace System.Xml.Xsl.Runtime
{
    using System;

    internal class XmlDoubleSortKey : XmlSortKey
    {
        private double dblVal;
        private bool isNaN;

        public XmlDoubleSortKey(double value, XmlCollation collation)
        {
            if (double.IsNaN(value))
            {
                this.isNaN = true;
                this.dblVal = (collation.EmptyGreatest != collation.DescendingOrder) ? double.PositiveInfinity : double.NegativeInfinity;
            }
            else
            {
                this.dblVal = collation.DescendingOrder ? -value : value;
            }
        }

        public override int CompareTo(object obj)
        {
            XmlDoubleSortKey that = obj as XmlDoubleSortKey;
            if (that == null)
            {
                if (this.isNaN)
                {
                    return base.BreakSortingTie(obj as XmlSortKey);
                }
                return base.CompareToEmpty(obj);
            }
            if (this.dblVal == that.dblVal)
            {
                if (this.isNaN)
                {
                    if (that.isNaN)
                    {
                        return base.BreakSortingTie(that);
                    }
                    if (this.dblVal != double.NegativeInfinity)
                    {
                        return 1;
                    }
                    return -1;
                }
                if (!that.isNaN)
                {
                    return base.BreakSortingTie(that);
                }
                if (that.dblVal != double.NegativeInfinity)
                {
                    return -1;
                }
                return 1;
            }
            if (this.dblVal >= that.dblVal)
            {
                return 1;
            }
            return -1;
        }
    }
}

