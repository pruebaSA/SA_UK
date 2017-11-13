namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using System.Xml;

    internal abstract class WsrmFault : MessageFault
    {
        private FaultCode code;
        private string exceptionMessage;
        private bool hasDetail;
        private bool isRemote;
        private FaultReason reason;
        private ReliableMessagingVersion reliableMessagingVersion;
        private string subcode;

        protected WsrmFault(FaultCode code, string subcode, FaultReason reason)
        {
            this.code = code;
            this.subcode = subcode;
            this.reason = reason;
            this.isRemote = true;
        }

        protected WsrmFault(bool isSenderFault, string subcode, string faultReason, string exceptionMessage)
        {
            if (isSenderFault)
            {
                this.code = new FaultCode("Sender", "");
            }
            else
            {
                this.code = new FaultCode("Receiver", "");
            }
            this.subcode = subcode;
            this.reason = new FaultReason(faultReason, CultureInfo.CurrentCulture);
            this.exceptionMessage = exceptionMessage;
            this.isRemote = false;
        }

        public virtual CommunicationException CreateException()
        {
            string safeReasonText;
            if (this.IsRemote)
            {
                safeReasonText = FaultException.GetSafeReasonText(this.reason);
                safeReasonText = System.ServiceModel.SR.GetString("WsrmFaultReceived", new object[] { safeReasonText });
            }
            else
            {
                if (this.exceptionMessage == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                safeReasonText = this.exceptionMessage;
            }
            if (this.code.IsSenderFault)
            {
                return new ProtocolException(safeReasonText);
            }
            return new CommunicationException(safeReasonText);
        }

        public static CommunicationException CreateException(WsrmFault fault) => 
            fault.CreateException();

        public Message CreateMessage(MessageVersion messageVersion, ReliableMessagingVersion reliableMessagingVersion)
        {
            this.SetReliableMessagingVersion(reliableMessagingVersion);
            string faultActionString = WsrmIndex.GetFaultActionString(messageVersion.Addressing, reliableMessagingVersion);
            if (messageVersion.Envelope == EnvelopeVersion.Soap11)
            {
                this.code = this.Get11Code(this.code, this.subcode);
            }
            else
            {
                if (messageVersion.Envelope != EnvelopeVersion.Soap12)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                if (this.code.SubCode == null)
                {
                    FaultCode subCode = new FaultCode(this.subcode, WsrmIndex.GetNamespaceString(reliableMessagingVersion));
                    this.code = new FaultCode(this.code.Name, this.code.Namespace, subCode);
                }
                this.hasDetail = this.Get12HasDetail();
            }
            Message message = Message.CreateMessage(messageVersion, this, faultActionString);
            this.OnFaultMessageCreated(messageVersion, message);
            return message;
        }

        protected abstract FaultCode Get11Code(FaultCode code, string subcode);
        protected abstract bool Get12HasDetail();
        protected string GetExceptionMessage()
        {
            if (this.exceptionMessage == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            return this.exceptionMessage;
        }

        protected ReliableMessagingVersion GetReliableMessagingVersion()
        {
            if (this.reliableMessagingVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            return this.reliableMessagingVersion;
        }

        protected abstract void OnFaultMessageCreated(MessageVersion version, Message message);
        protected void SetReliableMessagingVersion(ReliableMessagingVersion reliableMessagingVersion)
        {
            if (reliableMessagingVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            if (this.reliableMessagingVersion != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            this.reliableMessagingVersion = reliableMessagingVersion;
        }

        internal void WriteDetail(XmlDictionaryWriter writer)
        {
            this.OnWriteDetailContents(writer);
        }

        public override FaultCode Code =>
            this.code;

        public override bool HasDetail =>
            this.hasDetail;

        public bool IsRemote =>
            this.isRemote;

        public override FaultReason Reason =>
            this.reason;

        public string Subcode =>
            this.subcode;
    }
}

