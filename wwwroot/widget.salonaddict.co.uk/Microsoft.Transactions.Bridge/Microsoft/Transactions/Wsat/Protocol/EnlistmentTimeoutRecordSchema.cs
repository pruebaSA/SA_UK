namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="EnlistmentTimeout")]
    internal class EnlistmentTimeoutRecordSchema : TraceRecord
    {
        [DataMember(Name="EnlistmentId", IsRequired=true)]
        private Guid enlistmentId;
        private TransactionOutcome outcome;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/EnlistmentTimeoutTraceRecord";
        private TimeSpan timeout;
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        public EnlistmentTimeoutRecordSchema(string transactionId, Guid enlistmentId, TransactionOutcome outcome, TimeSpan timeout)
        {
            this.transactionId = transactionId;
            this.enlistmentId = enlistmentId;
            this.outcome = outcome;
            this.timeout = timeout;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/EnlistmentTimeoutTraceRecord";

        [DataMember(Name="Outcome", IsRequired=true)]
        private string Outcome
        {
            get => 
                this.outcome.ToString();
            set
            {
            }
        }

        [DataMember(Name="Timeout", IsRequired=true)]
        private string Timeout
        {
            get => 
                this.timeout.ToString();
            set
            {
            }
        }
    }
}

