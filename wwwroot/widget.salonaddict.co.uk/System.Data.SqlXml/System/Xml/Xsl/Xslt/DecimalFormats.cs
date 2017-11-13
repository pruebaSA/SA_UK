namespace System.Xml.Xsl.Xslt
{
    using System.Collections.ObjectModel;
    using System.Xml;

    internal class DecimalFormats : KeyedCollection<XmlQualifiedName, DecimalFormatDecl>
    {
        protected override XmlQualifiedName GetKeyForItem(DecimalFormatDecl format) => 
            format.Name;
    }
}

