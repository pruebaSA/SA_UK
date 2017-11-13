namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class WsrmRequiredFault : WsrmFault
    {
        public WsrmRequiredFault(string faultReason) : base(true, "WsrmRequired", faultReason, null)
        {
        }

        protected override FaultCode Get11Code(FaultCode code, string subcode) => 
            new FaultCode(subcode, WsrmIndex.GetNamespaceString(base.GetReliableMessagingVersion()));

        protected override bool Get12HasDetail() => 
            false;

        protected override void OnFaultMessageCreated(MessageVersion version, Message message)
        {
        }

        protected override void OnWriteDetailContents(XmlDictionaryWriter writer)
        {
        }
    }
}

