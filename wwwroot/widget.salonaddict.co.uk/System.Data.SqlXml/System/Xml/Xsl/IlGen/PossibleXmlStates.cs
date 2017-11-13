namespace System.Xml.Xsl.IlGen
{
    using System;

    internal enum PossibleXmlStates
    {
        None,
        WithinSequence,
        EnumAttrs,
        WithinContent,
        WithinAttr,
        WithinComment,
        WithinPI,
        Any
    }
}

