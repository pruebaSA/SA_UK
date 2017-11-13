namespace System.Xml.Xsl.XsltOld.Debugger
{
    using System;

    internal interface IXsltProcessor
    {
        IStackFrame GetStackFrame(int depth);

        int StackDepth { get; }
    }
}

