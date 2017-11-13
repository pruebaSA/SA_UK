namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="RegisterCoordinator")]
    internal abstract class RegisterCoordinatorRecordSchema : TraceRecord
    {
        [DataMember(Name="Context", IsRequired=true)]
        private CoordinationContext context;
        private ControlProtocol protocol;
        protected string schemaId;

        protected RegisterCoordinatorRecordSchema(CoordinationContext context, ControlProtocol protocol)
        {
            this.context = context;
            this.protocol = protocol;
        }

        public static RegisterCoordinatorRecordSchema Instance(CoordinationContext context, ControlProtocol protocol, EndpointAddress coordinatorService, ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(RegisterCoordinatorRecordSchema), "Instance");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return new RegisterCoordinatorRecordSchema10(context, protocol, coordinatorService);

                case ProtocolVersion.Version11:
                    return new RegisterCoordinatorRecordSchema11(context, protocol, coordinatorService);
            }
            return null;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            this.schemaId;

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

