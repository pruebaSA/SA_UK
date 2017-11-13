namespace MS.Internal.Xml.XPath
{
    using System;
    using System.Globalization;
    using System.Xml;
    using System.Xml.XPath;

    internal sealed class OperandQuery : ValueQuery
    {
        internal object val;

        public OperandQuery(object val)
        {
            this.val = val;
        }

        public override XPathNodeIterator Clone() => 
            this;

        public override object Evaluate(XPathNodeIterator nodeIterator) => 
            this.val;

        public override void PrintQuery(XmlWriter w)
        {
            w.WriteStartElement(base.GetType().Name);
            w.WriteAttributeString("value", Convert.ToString(this.val, CultureInfo.InvariantCulture));
            w.WriteEndElement();
        }

        public override XPathResultType StaticType =>
            base.GetXPathType(this.val);
    }
}

