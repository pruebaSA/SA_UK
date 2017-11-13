namespace System.Xml.Xsl
{
    using System;

    internal interface ISourceLineInfo
    {
        int EndLine { get; }

        int EndPos { get; }

        bool IsNoSource { get; }

        int StartLine { get; }

        int StartPos { get; }

        string Uri { get; }
    }
}

