namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Xsl.Qil;

    internal class Template : ProtoTemplate
    {
        public int ImportPrecedence;
        public readonly string Match;
        public readonly QilName Mode;
        public int OrderNumber;
        public readonly double Priority;

        public Template(QilName name, string match, QilName mode, double priority, XslVersion xslVer) : base(XslNodeType.Template, name, xslVer)
        {
            this.Match = match;
            this.Mode = mode;
            this.Priority = priority;
        }

        public override string GetDebugName()
        {
            BufferBuilder builder = new BufferBuilder();
            builder.Append("<xsl:template");
            if (this.Match != null)
            {
                builder.Append(" match=\"");
                builder.Append(this.Match);
                builder.Append('"');
            }
            if (base.Name != null)
            {
                builder.Append(" name=\"");
                builder.Append(base.Name.QualifiedName);
                builder.Append('"');
            }
            if (!double.IsNaN(this.Priority))
            {
                builder.Append(" priority=\"");
                builder.Append(this.Priority.ToString(CultureInfo.InvariantCulture));
                builder.Append('"');
            }
            if (this.Mode.LocalName.Length != 0)
            {
                builder.Append(" mode=\"");
                builder.Append(this.Mode.QualifiedName);
                builder.Append('"');
            }
            builder.Append('>');
            return builder.ToString();
        }
    }
}

