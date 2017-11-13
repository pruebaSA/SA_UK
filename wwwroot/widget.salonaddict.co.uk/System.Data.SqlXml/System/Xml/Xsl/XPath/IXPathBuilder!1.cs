namespace System.Xml.Xsl.XPath
{
    using System;
    using System.Collections.Generic;
    using System.Xml.XPath;

    internal interface IXPathBuilder<Node>
    {
        Node Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name);
        Node EndBuild(Node result);
        Node Function(string prefix, string name, IList<Node> args);
        Node JoinStep(Node left, Node right);
        Node Number(double value);
        Node Operator(XPathOperator op, Node left, Node right);
        Node Predicate(Node node, Node condition, bool reverseStep);
        void StartBuild();
        Node String(string value);
        Node Variable(string prefix, string name);
    }
}

