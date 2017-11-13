namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Pattern
    {
        public readonly TemplateMatch Match;
        public readonly int Priority;
        public Pattern(TemplateMatch match, int priority)
        {
            this.Match = match;
            this.Priority = priority;
        }
    }
}

