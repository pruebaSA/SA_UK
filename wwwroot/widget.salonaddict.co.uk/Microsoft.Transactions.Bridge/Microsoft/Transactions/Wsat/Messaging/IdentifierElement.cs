namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    internal abstract class IdentifierElement : IXmlSerializable
    {
        private CoordinationStrings coordinationStrings;
        private string identifier;

        public IdentifierElement(ProtocolVersion protocolVersion) : this(protocolVersion, null)
        {
        }

        public IdentifierElement(ProtocolVersion protocolVersion, string identifier)
        {
            this.identifier = identifier;
            this.coordinationStrings = CoordinationStrings.Version(protocolVersion);
        }

        public static IdentifierElement Instance(string identifier, ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(IdentifierElement), "V");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return new IdentifierElement10(identifier);

                case ProtocolVersion.Version11:
                    return new IdentifierElement11(identifier);
            }
            return null;
        }

        XmlSchema IXmlSerializable.GetSchema() => 
            null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.identifier = reader.ReadElementString(this.coordinationStrings.Identifier, this.coordinationStrings.Namespace).Trim();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteValue(this.identifier);
        }

        public string Identifier =>
            this.identifier;
    }
}

