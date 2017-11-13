namespace System.Xml
{
    using System;

    internal class XmlNullResolver : XmlUrlResolver
    {
        public static readonly XmlNullResolver Singleton = new XmlNullResolver();

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            throw new XmlException("Xml_NullResolver", string.Empty);
        }
    }
}

