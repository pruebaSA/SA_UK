﻿namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="CoordinatorOutcome")]
    internal class CoordinatorOutcomeRecordSchema : TraceRecord
    {
        private TransactionOutcome outcome;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/CoordinatorOutcomeTraceRecord";
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        public CoordinatorOutcomeRecordSchema(string transactionId, TransactionOutcome outcome)
        {
            this.transactionId = transactionId;
            this.outcome = outcome;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/CoordinatorOutcomeTraceRecord";

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

