namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Xsl;

    internal sealed class PrefixQName
    {
        public string Name;
        public string Namespace;
        public string Prefix;

        internal void ClearPrefix()
        {
            this.Prefix = string.Empty;
        }

        private static string ParseNCName(string qname, ref int position)
        {
            int length = qname.Length;
            int startIndex = position;
            XmlCharType instance = XmlCharType.Instance;
            if ((length == position) || !instance.IsStartNCNameChar(qname[position]))
            {
                throw XsltException.Create("Xslt_InvalidQName", new string[] { qname });
            }
            position++;
            while ((position < length) && instance.IsNCNameChar(qname[position]))
            {
                position++;
            }
            return qname.Substring(startIndex, position - startIndex);
        }

        public static void ParseQualifiedName(string qname, out string prefix, out string local)
        {
            prefix = string.Empty;
            local = string.Empty;
            int position = 0;
            local = ParseNCName(qname, ref position);
            if (position < qname.Length)
            {
                if (qname[position] == ':')
                {
                    position++;
                    prefix = local;
                    local = ParseNCName(qname, ref position);
                }
                if (position < qname.Length)
                {
                    throw XsltException.Create("Xslt_InvalidQName", new string[] { qname });
                }
            }
        }

        internal void SetQName(string qname)
        {
            ParseQualifiedName(qname, out this.Prefix, out this.Name);
        }

        public static bool ValidatePrefix(string prefix)
        {
            if (prefix.Length == 0)
            {
                return false;
            }
            XmlCharType instance = XmlCharType.Instance;
            if (!instance.IsStartNCNameChar(prefix[0]))
            {
                return false;
            }
            for (int i = 1; i < prefix.Length; i++)
            {
                if (!instance.IsNCNameChar(prefix[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

