namespace System.Xml.Xsl.XsltOld.Debugger
{
    using System;
    using System.Xml.XPath;

    internal interface IStackFrame
    {
        XPathNavigator GetVariable(int varIndex);
        int GetVariablesCount();
        object GetVariableValue(int varIndex);

        XPathNavigator Instruction { get; }

        XPathNodeIterator NodeSet { get; }
    }
}

