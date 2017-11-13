namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="UnhandledStateMachineException")]
    internal class UnhandledStateMachineExceptionRecordSchema : TraceRecord
    {
        [DataMember(Name="CurrentState", IsRequired=true)]
        private string currentState;
        [DataMember(Name="TransitionHistory")]
        private StateMachineHistory history;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/UnhandledStateMachineExceptionTraceRecord";
        [DataMember(Name="StateMachine", IsRequired=true)]
        private string stateMachine;
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        public UnhandledStateMachineExceptionRecordSchema(string transactionId, string stateMachine, string currentState, StateMachineHistory history)
        {
            this.transactionId = transactionId;
            this.stateMachine = stateMachine;
            this.currentState = currentState;
            this.history = history;
        }

        public override string ToString() => 
            Microsoft.Transactions.SR.GetString("UnhandledStateMachineExceptionRecordSchema", new object[] { this.transactionId, this.stateMachine, this.currentState, (this.history != null) ? this.history.ToString() : string.Empty });

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/UnhandledStateMachineExceptionTraceRecord";
    }
}

