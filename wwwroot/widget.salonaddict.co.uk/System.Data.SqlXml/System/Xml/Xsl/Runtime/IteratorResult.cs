namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum IteratorResult
    {
        NoMoreNodes,
        NeedInputNode,
        HaveCurrentNode
    }
}

