namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml;
    using System.Xml.Xsl.Qil;

    internal class Key : XslNode
    {
        public QilFunction Function;
        public readonly string Match;
        public readonly string Use;

        public Key(QilName name, string match, string use, XslVersion xslVer) : base(XslNodeType.Key, name, null, xslVer)
        {
            this.Match = match;
            this.Use = use;
        }

        public string GetDebugName()
        {
            BufferBuilder builder = new BufferBuilder();
            builder.Append("<xsl:key name=\"");
            builder.Append(base.Name.QualifiedName);
            builder.Append('"');
            if (this.Match != null)
            {
                builder.Append(" match=\"");
                builder.Append(this.Match);
                builder.Append('"');
            }
            if (this.Use != null)
            {
                builder.Append(" use=\"");
                builder.Append(this.Use);
                builder.Append('"');
            }
            builder.Append('>');
            return builder.ToString();
        }
    }
}

