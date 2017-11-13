namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="ParticipantOutcome")]
    internal class ParticipantOutcomeRecordSchema : TraceRecord
    {
        [DataMember(Name="EnlistmentId", IsRequired=true)]
        private Guid enlistmentId;
        private TransactionOutcome outcome;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/ParticipantOutcomeTraceRecord";
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        public ParticipantOutcomeRecordSchema(string transactionId, Guid enlistmentId, TransactionOutcome outcome)
        {
            this.transactionId = transactionId;
            this.enlistmentId = enlistmentId;
            this.outcome = outcome;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/ParticipantOutcomeTraceRecord";

        [DataMember(Name="Outcome", IsRequired=true)]
        private string Outcome
        {
            get => 
                this.outcome.ToString();
            set
            {
            }
        }
    }
}

