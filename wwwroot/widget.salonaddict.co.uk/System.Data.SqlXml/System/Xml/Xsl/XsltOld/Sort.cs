namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal class Sort
    {
        internal XmlCaseOrder caseOrder;
        internal XmlDataType dataType;
        internal string lang;
        internal XmlSortOrder order;
        internal int select;

        public Sort(int sortkey, string xmllang, XmlDataType datatype, XmlSortOrder xmlorder, XmlCaseOrder xmlcaseorder)
        {
            this.select = sortkey;
            this.lang = xmllang;
            this.dataType = datatype;
            this.order = xmlorder;
            this.caseOrder = xmlcaseorder;
        }
    }
}

