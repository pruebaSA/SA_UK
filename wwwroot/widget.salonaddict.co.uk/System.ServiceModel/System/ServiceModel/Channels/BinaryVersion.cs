namespace System.ServiceModel.Channels
{
    using System;
    using System.Xml;

    internal class BinaryVersion
    {
        private string contentType;
        private IXmlDictionary dictionary;
        private string sessionContentType;
        public static readonly BinaryVersion Version1 = new BinaryVersion("application/soap+msbin1", "application/soap+msbinsession1", ServiceModelDictionary.Version1);

        private BinaryVersion(string contentType, string sessionContentType, IXmlDictionary dictionary)
        {
            this.contentType = contentType;
            this.sessionContentType = sessionContentType;
            this.dictionary = dictionary;
        }

        public string ContentType =>
            this.contentType;

        public static BinaryVersion CurrentVersion =>
            Version1;

        public IXmlDictionary Dictionary =>
            this.dictionary;

        public string SessionContentType =>
            this.sessionContentType;
    }
}

