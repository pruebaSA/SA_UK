namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Threading;
    using System.Xml;
    using System.Xml.XPath;

    internal sealed class XPathComparerHelper : IComparer
    {
        private XmlCaseOrder caseOrder;
        private CultureInfo cinfo;
        private XmlDataType dataType;
        private XmlSortOrder order;

        public XPathComparerHelper(XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
        {
            if (lang == null)
            {
                this.cinfo = Thread.CurrentThread.CurrentCulture;
            }
            else
            {
                try
                {
                    this.cinfo = new CultureInfo(lang);
                }
                catch (ArgumentException)
                {
                    throw;
                }
            }
            if (order == XmlSortOrder.Descending)
            {
                if (caseOrder == XmlCaseOrder.LowerFirst)
                {
                    caseOrder = XmlCaseOrder.UpperFirst;
                }
                else if (caseOrder == XmlCaseOrder.UpperFirst)
                {
                    caseOrder = XmlCaseOrder.LowerFirst;
                }
            }
            this.order = order;
            this.caseOrder = caseOrder;
            this.dataType = dataType;
        }

        public int Compare(object x, object y)
        {
            int num = (this.order == XmlSortOrder.Ascending) ? 1 : -1;
            switch (this.dataType)
            {
                case XmlDataType.Text:
                {
                    string strA = Convert.ToString(x, this.cinfo);
                    string strB = Convert.ToString(y, this.cinfo);
                    int num2 = string.Compare(strA, strB, this.caseOrder != XmlCaseOrder.None, this.cinfo);
                    if ((num2 == 0) && (this.caseOrder != XmlCaseOrder.None))
                    {
                        int num3 = (this.caseOrder == XmlCaseOrder.LowerFirst) ? 1 : -1;
                        num2 = string.Compare(strA, strB, false, this.cinfo);
                        return (num3 * num2);
                    }
                    return (num * num2);
                }
                case XmlDataType.Number:
                {
                    double d = XmlConvert.ToXPathDouble(x);
                    double num5 = XmlConvert.ToXPathDouble(y);
                    if (d <= num5)
                    {
                        if (d >= num5)
                        {
                            if (d == num5)
                            {
                                return 0;
                            }
                            if (!double.IsNaN(d))
                            {
                                return num;
                            }
                            if (double.IsNaN(num5))
                            {
                                return 0;
                            }
                        }
                        return (-1 * num);
                    }
                    return num;
                }
            }
            throw new InvalidOperationException(Res.GetString("Xml_InvalidOperation"));
        }
    }
}

