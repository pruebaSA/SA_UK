namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(ElementName="Identifier", Namespace="http://docs.oasis-open.org/ws-tx/wscoor/2006/06")]
    internal class IdentifierElement11 : IdentifierElement
    {
        public IdentifierElement11() : this(null)
        {
        }

        public IdentifierElement11(string identifier) : base(ProtocolVersion.Version11, identifier)
        {
        }
    }
}

