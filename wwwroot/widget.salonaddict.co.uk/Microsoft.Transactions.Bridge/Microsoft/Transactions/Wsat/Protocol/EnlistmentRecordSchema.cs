namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="Enlistment")]
    internal class EnlistmentRecordSchema : TraceRecord
    {
        [DataMember(Name="EnlistmentId", IsRequired=true)]
        private Guid enlistmentId;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/EnlistmentTraceRecord";
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        public EnlistmentRecordSchema(string transactionId, Guid enlistmentId)
        {
            this.transactionId = transactionId;
            this.enlistmentId = enlistmentId;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/EnlistmentTraceRecord";
    }
}

