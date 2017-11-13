namespace System.Web.Services.Protocols
{
    using System;
    using System.Runtime.InteropServices;

    public sealed class SoapServerMessage : SoapMessage
    {
        internal SoapExtension[] allExtensions;
        internal SoapExtension[] highPriConfigExtensions;
        internal SoapExtension[] otherExtensions;
        private SoapServerProtocol protocol;

        internal SoapServerMessage(SoapServerProtocol protocol)
        {
            this.protocol = protocol;
        }

        protected override void EnsureInStage()
        {
            base.EnsureStage(SoapMessageStage.AfterDeserialize);
        }

        protected override void EnsureOutStage()
        {
            base.EnsureStage(SoapMessageStage.BeforeSerialize);
        }

        public override string Action =>
            this.protocol.ServerMethod.action;

        public override LogicalMethodInfo MethodInfo =>
            this.protocol.MethodInfo;

        public override bool OneWay =>
            this.protocol.ServerMethod.oneWay;

        public object Server
        {
            get
            {
                base.EnsureStage(SoapMessageStage.AfterDeserialize | SoapMessageStage.BeforeSerialize);
                return this.protocol.Target;
            }
        }

        [ComVisible(false)]
        public override SoapProtocolVersion SoapVersion =>
            this.protocol.Version;

        public override string Url =>
            this.protocol.Request.Url.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped).Replace("#", "%23");
    }
}

