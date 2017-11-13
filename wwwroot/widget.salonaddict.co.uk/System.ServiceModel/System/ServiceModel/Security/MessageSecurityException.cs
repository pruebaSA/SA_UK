namespace System.ServiceModel.Security
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    [Serializable]
    public class MessageSecurityException : CommunicationException
    {
        private MessageFault fault;
        private bool isReplay;

        public MessageSecurityException()
        {
        }

        public MessageSecurityException(string message) : base(message)
        {
        }

        protected MessageSecurityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal MessageSecurityException(string message, bool isReplay) : base(message)
        {
            this.isReplay = isReplay;
        }

        public MessageSecurityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal MessageSecurityException(string message, Exception innerException, MessageFault fault) : base(message, innerException)
        {
            this.fault = fault;
        }

        [SecurityCritical, SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        internal MessageFault Fault =>
            this.fault;

        internal bool ReplayDetected =>
            this.isReplay;
    }
}

