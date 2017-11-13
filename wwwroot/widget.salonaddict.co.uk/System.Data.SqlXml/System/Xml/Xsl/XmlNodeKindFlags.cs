namespace System.Xml.Xsl
{
    using System;

    [Flags]
    internal enum XmlNodeKindFlags
    {
        Any = 0x7f,
        Attribute = 4,
        Comment = 0x10,
        Content = 0x3a,
        Document = 1,
        Element = 2,
        Namespace = 0x40,
        None = 0,
        PI = 0x20,
        Text = 8
    }
}

