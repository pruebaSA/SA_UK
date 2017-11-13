namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="CoordinationContext")]
    internal class CoordinationContextRecordSchema : TraceRecord
    {
        [DataMember(Name="Context", IsRequired=true)]
        private CoordinationContext context;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/CoordinationContextTraceRecord";

        public CoordinationContextRecordSchema(CoordinationContext context)
        {
            this.context = context;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/CoordinationContextTraceRecord";
    }
}

