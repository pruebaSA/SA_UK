namespace System.Xml.Xsl.Runtime
{
    using System;

    internal class XmlDateTimeSortKey : XmlIntegerSortKey
    {
        public XmlDateTimeSortKey(DateTime value, XmlCollation collation) : base(value.Ticks, collation)
        {
        }
    }
}

