namespace System.Xml.Xsl.Xslt
{
    using System;

    internal class NsAlias
    {
        public readonly int ImportPrecedence;
        public readonly string ResultNsUri;
        public readonly string ResultPrefix;

        public NsAlias(string resultNsUri, string resultPrefix, int importPrecedence)
        {
            this.ResultNsUri = resultNsUri;
            this.ResultPrefix = resultPrefix;
            this.ImportPrecedence = importPrecedence;
        }
    }
}

