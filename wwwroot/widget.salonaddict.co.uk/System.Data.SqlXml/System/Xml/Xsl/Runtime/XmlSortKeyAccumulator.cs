namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct XmlSortKeyAccumulator
    {
        private const int DefaultSortKeyCount = 0x40;
        private XmlSortKey[] keys;
        private int pos;
        public void Create()
        {
            if (this.keys == null)
            {
                this.keys = new XmlSortKey[0x40];
            }
            this.pos = 0;
            this.keys[0] = null;
        }

        public void AddStringSortKey(XmlCollation collation, string value)
        {
            this.AppendSortKey(collation.CreateSortKey(value));
        }

        public void AddDecimalSortKey(XmlCollation collation, decimal value)
        {
            this.AppendSortKey(new XmlDecimalSortKey(value, collation));
        }

        public void AddIntegerSortKey(XmlCollation collation, long value)
        {
            this.AppendSortKey(new XmlIntegerSortKey(value, collation));
        }

        public void AddIntSortKey(XmlCollation collation, int value)
        {
            this.AppendSortKey(new XmlIntSortKey(value, collation));
        }

        public void AddDoubleSortKey(XmlCollation collation, double value)
        {
            this.AppendSortKey(new XmlDoubleSortKey(value, collation));
        }

        public void AddDateTimeSortKey(XmlCollation collation, DateTime value)
        {
            this.AppendSortKey(new XmlDateTimeSortKey(value, collation));
        }

        public void AddEmptySortKey(XmlCollation collation)
        {
            this.AppendSortKey(new XmlEmptySortKey(collation));
        }

        public void FinishSortKeys()
        {
            this.pos++;
            if (this.pos >= this.keys.Length)
            {
                XmlSortKey[] destinationArray = new XmlSortKey[this.pos * 2];
                Array.Copy(this.keys, 0, destinationArray, 0, this.keys.Length);
                this.keys = destinationArray;
            }
            this.keys[this.pos] = null;
        }

        private void AppendSortKey(XmlSortKey key)
        {
            key.Priority = this.pos;
            if (this.keys[this.pos] == null)
            {
                this.keys[this.pos] = key;
            }
            else
            {
                this.keys[this.pos].AddSortKey(key);
            }
        }

        public Array Keys =>
            this.keys;
    }
}

