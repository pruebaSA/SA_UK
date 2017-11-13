namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml.Xsl.Qil;

    internal class Text : XslNode
    {
        public readonly SerializationHints Hints;

        public Text(string data, SerializationHints hints, XslVersion xslVer) : base(XslNodeType.Text, null, data, xslVer)
        {
            this.Hints = hints;
        }
    }
}

