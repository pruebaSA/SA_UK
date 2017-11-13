namespace System.Xml.Xsl.Runtime
{
    using System;

    internal enum XmlState
    {
        WithinSequence,
        EnumAttrs,
        WithinContent,
        WithinAttr,
        WithinNmsp,
        WithinComment,
        WithinPI
    }
}

