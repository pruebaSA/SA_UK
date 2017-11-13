namespace System.ServiceModel.Syndication
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    public class UrlSyndicationContent : SyndicationContent
    {
        private string mediaType;
        private Uri url;

        protected UrlSyndicationContent(UrlSyndicationContent source) : base(source)
        {
            if (source == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            this.url = source.url;
            this.mediaType = source.mediaType;
        }

        public UrlSyndicationContent(Uri url, string mediaType)
        {
            if (url == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("url");
            }
            this.url = url;
            this.mediaType = mediaType;
        }

        public override SyndicationContent Clone() => 
            new UrlSyndicationContent(this);

        protected override void WriteContentsTo(XmlWriter writer)
        {
            writer.WriteAttributeString("src", string.Empty, FeedUtils.GetUriString(this.url));
        }

        public override string Type =>
            this.mediaType;

        public Uri Url =>
            this.url;
    }
}

