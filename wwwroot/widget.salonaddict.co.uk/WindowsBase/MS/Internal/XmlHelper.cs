namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Xml;

    [FriendAccessAllowed]
    internal static class XmlHelper
    {
        internal static string ExtractString(XmlNode node)
        {
            string str = "";
            if (node.NodeType == XmlNodeType.Element)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    if (node.ChildNodes[i].NodeType == XmlNodeType.Text)
                    {
                        str = str + node.ChildNodes[i].Value;
                    }
                }
                return str;
            }
            return node.Value;
        }

        internal static bool IsXmlNode(object item) => 
            (item?.GetType().FullName.StartsWith("System.Xml", StringComparison.Ordinal) && IsXmlNodeHelper(item));

        private static bool IsXmlNodeHelper(object item) => 
            (item is XmlNode);

        internal static string SelectStringValue(XmlNode node, string query) => 
            SelectStringValue(node, query, null);

        internal static string SelectStringValue(XmlNode node, string query, XmlNamespaceManager namespaceManager)
        {
            XmlNode node2 = node.SelectSingleNode(query, namespaceManager);
            if (node2 != null)
            {
                return ExtractString(node2);
            }
            return string.Empty;
        }
    }
}

