namespace System.Xml.Xsl.XsltOld.Debugger
{
    using System;
    using System.Xml.XPath;

    internal interface IXsltDebugger
    {
        string GetBuiltInTemplatesUri();
        void OnInstructionCompile(XPathNavigator styleSheetNavigator);
        void OnInstructionExecute(IXsltProcessor xsltProcessor);
    }
}

