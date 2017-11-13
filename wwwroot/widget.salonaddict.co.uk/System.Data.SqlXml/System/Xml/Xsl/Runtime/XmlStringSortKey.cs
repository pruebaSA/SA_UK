namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Globalization;

    internal class XmlStringSortKey : XmlSortKey
    {
        private bool descendingOrder;
        private SortKey sortKey;
        private byte[] sortKeyBytes;

        public XmlStringSortKey(SortKey sortKey, bool descendingOrder)
        {
            this.sortKey = sortKey;
            this.descendingOrder = descendingOrder;
        }

        public XmlStringSortKey(byte[] sortKey, bool descendingOrder)
        {
            this.sortKeyBytes = sortKey;
            this.descendingOrder = descendingOrder;
        }

        public override int CompareTo(object obj)
        {
            int num3;
            XmlStringSortKey that = obj as XmlStringSortKey;
            if (that == null)
            {
                return base.CompareToEmpty(obj);
            }
            if (this.sortKey != null)
            {
                num3 = SortKey.Compare(this.sortKey, that.sortKey);
            }
            else
            {
                int num2 = (this.sortKeyBytes.Length < that.sortKeyBytes.Length) ? this.sortKeyBytes.Length : that.sortKeyBytes.Length;
                for (int i = 0; i < num2; i++)
                {
                    if (this.sortKeyBytes[i] < that.sortKeyBytes[i])
                    {
                        num3 = -1;
                        goto Label_00BC;
                    }
                    if (this.sortKeyBytes[i] > that.sortKeyBytes[i])
                    {
                        num3 = 1;
                        goto Label_00BC;
                    }
                }
                if (this.sortKeyBytes.Length < that.sortKeyBytes.Length)
                {
                    num3 = -1;
                }
                else if (this.sortKeyBytes.Length > that.sortKeyBytes.Length)
                {
                    num3 = 1;
                }
                else
                {
                    num3 = 0;
                }
            }
        Label_00BC:
            if (num3 == 0)
            {
                return base.BreakSortingTie(that);
            }
            if (!this.descendingOrder)
            {
                return num3;
            }
            return -num3;
        }
    }
}

