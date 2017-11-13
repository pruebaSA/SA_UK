namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum SetIteratorResult
    {
        NoMoreNodes,
        InitRightIterator,
        NeedLeftNode,
        NeedRightNode,
        HaveCurrentNode
    }
}

