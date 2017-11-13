namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="ProtocolService")]
    internal class ProtocolServiceRecordSchema : TraceRecord
    {
        [DataMember(Name="ProtocolIdentifier", IsRequired=true)]
        private Guid protocolId;
        [DataMember(Name="ProtocolName", IsRequired=true)]
        private string protocolName;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/ProtocolServiceTraceRecord";

        public ProtocolServiceRecordSchema(string protocolName, Guid protocolId)
        {
            this.protocolName = protocolName;
            this.protocolId = protocolId;
        }

        public override string ToString() => 
            Microsoft.Transactions.SR.GetString("ProtocolServiceRecordSchema", new object[] { this.protocolName, this.protocolId.ToString() });

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/ProtocolServiceTraceRecord";
    }
}

