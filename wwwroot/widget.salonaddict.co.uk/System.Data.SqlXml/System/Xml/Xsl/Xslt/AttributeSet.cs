namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Xsl.Qil;

    internal class AttributeSet : ProtoTemplate
    {
        public System.Xml.Xsl.Xslt.CycleCheck CycleCheck;
        public readonly List<QilName> UsedAttributeSets;

        public AttributeSet(QilName name, XslVersion xslVer) : base(XslNodeType.AttributeSet, name, xslVer)
        {
            this.UsedAttributeSets = new List<QilName>();
        }

        public void AddContent(XslNode node)
        {
            base.AddContent(node);
        }

        public override string GetDebugName()
        {
            BufferBuilder builder = new BufferBuilder();
            builder.Append("<xsl:attribute-set name=\"");
            builder.Append(base.Name.QualifiedName);
            builder.Append("\">");
            return builder.ToString();
        }

        public void MergeContent(AttributeSet other)
        {
            this.UsedAttributeSets.InsertRange(0, other.UsedAttributeSets);
            base.InsertContent(other.Content);
        }
    }
}

