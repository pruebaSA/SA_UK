namespace System.ServiceModel.Channels
{
    using System;
    using System.Xml;

    internal class WebScriptMetadataMessage : BodyWriterMessage
    {
        private string proxyContent;
        private const string proxyContentTag = "JavaScriptProxy";

        public WebScriptMetadataMessage(string action, string proxyContent) : base(MessageVersion.None, action, new WebScriptMetadataBodyWriter(proxyContent))
        {
            this.proxyContent = proxyContent;
        }

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("JavaScriptProxy");
            writer.WriteCData(this.proxyContent);
            writer.WriteEndElement();
        }

        private class WebScriptMetadataBodyWriter : BodyWriter
        {
            private string proxyContent;

            public WebScriptMetadataBodyWriter(string proxyContent) : base(true)
            {
                this.proxyContent = proxyContent;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteRaw(this.proxyContent);
            }
        }
    }
}

