namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(ElementName="Identifier", Namespace="http://schemas.xmlsoap.org/ws/2004/10/wscoor")]
    internal class IdentifierElement10 : IdentifierElement
    {
        public IdentifierElement10() : this(null)
        {
        }

        public IdentifierElement10(string identifier) : base(ProtocolVersion.Version10, identifier)
        {
        }
    }
}

