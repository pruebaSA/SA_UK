namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Xml;

    internal class DecimalFormatDecl
    {
        public readonly char[] Characters;
        public static DecimalFormatDecl Default = new DecimalFormatDecl(new XmlQualifiedName(), "Infinity", "NaN", ".,%‰0#;-");
        public readonly string InfinitySymbol;
        public readonly XmlQualifiedName Name;
        public readonly string NanSymbol;

        public DecimalFormatDecl(XmlQualifiedName name, string infinitySymbol, string nanSymbol, string characters)
        {
            this.Name = name;
            this.InfinitySymbol = infinitySymbol;
            this.NanSymbol = nanSymbol;
            this.Characters = characters.ToCharArray();
        }
    }
}

