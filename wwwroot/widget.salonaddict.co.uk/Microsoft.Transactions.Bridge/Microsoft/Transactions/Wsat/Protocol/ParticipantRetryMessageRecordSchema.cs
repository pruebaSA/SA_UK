namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="ParticipantRetryMessage")]
    internal class ParticipantRetryMessageRecordSchema : TraceRecord
    {
        [DataMember(Name="RetryCount", IsRequired=true)]
        private int count;
        [DataMember(Name="EnlistmentId", IsRequired=true)]
        private Guid enlistmentId;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/ParticipantRetryMessageTraceRecord";
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        public ParticipantRetryMessageRecordSchema(string transactionId, Guid enlistmentId, int count)
        {
            this.transactionId = transactionId;
            this.enlistmentId = enlistmentId;
            this.count = count;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/ParticipantRetryMessageTraceRecord";
    }
}

