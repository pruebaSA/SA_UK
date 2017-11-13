namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="RegistrationCoordinatorFaulted")]
    internal class RegistrationCoordinatorFaultedSchema : TraceRecord
    {
        [DataMember(Name="Context", IsRequired=true)]
        private CoordinationContext context;
        private MessageFault fault;
        private ControlProtocol protocol;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/RegistrationCoordinatorFaultedTraceRecord";

        public RegistrationCoordinatorFaultedSchema(CoordinationContext context, ControlProtocol protocol, MessageFault fault)
        {
            this.context = context;
            this.protocol = protocol;
            this.fault = fault;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/RegistrationCoordinatorFaultedTraceRecord";

        [DataMember(Name="Fault")]
        private string Fault
        {
            get => 
                Library.GetFaultCodeName(this.fault);
            set
            {
            }
        }

        [DataMember(Name="Protocol", IsRequired=true)]
        private string Protocol
        {
            get => 
                this.protocol.ToString();
            set
            {
            }
        }
    }
}

