namespace System.Xml.Schema
{
    using System;

    public class ValidationEventArgs : EventArgs
    {
        private XmlSchemaException ex;
        private XmlSeverityType severity;

        internal ValidationEventArgs(XmlSchemaException ex)
        {
            this.ex = ex;
            this.severity = XmlSeverityType.Error;
        }

        internal ValidationEventArgs(XmlSchemaException ex, XmlSeverityType severity)
        {
            this.ex = ex;
            this.severity = severity;
        }

        public XmlSchemaException Exception =>
            this.ex;

        public string Message =>
            this.ex.Message;

        public XmlSeverityType Severity =>
            this.severity;
    }
}

