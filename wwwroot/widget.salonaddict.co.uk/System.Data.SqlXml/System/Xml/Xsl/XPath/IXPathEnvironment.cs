namespace System.Xml.Xsl.XPath
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Xsl.Qil;

    internal interface IXPathEnvironment : IFocus
    {
        QilNode ResolveFunction(string prefix, string name, IList<QilNode> args, IFocus env);
        string ResolvePrefix(string prefix);
        QilNode ResolveVariable(string prefix, string name);

        XPathQilFactory Factory { get; }
    }
}

