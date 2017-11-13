namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal class NamespaceInfo
    {
        internal string nameSpace;
        internal string prefix;
        internal int stylesheetId;

        internal NamespaceInfo(string prefix, string nameSpace, int stylesheetId)
        {
            this.prefix = prefix;
            this.nameSpace = nameSpace;
            this.stylesheetId = stylesheetId;
        }
    }
}

