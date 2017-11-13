namespace System.Xml.Xsl.Xslt
{
    using System;

    internal class Sort : XslNode
    {
        public readonly string CaseOrder;
        public readonly string DataType;
        public readonly string Lang;
        public readonly string Order;

        public Sort(string select, string lang, string dataType, string order, string caseOrder, XslVersion xslVer) : base(XslNodeType.Sort, null, select, xslVer)
        {
            this.Lang = lang;
            this.DataType = dataType;
            this.Order = order;
            this.CaseOrder = caseOrder;
        }
    }
}

