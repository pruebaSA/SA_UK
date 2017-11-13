namespace System.Xml.Xsl
{
    using System;
    using System.Xml;

    internal class XmlQualifiedNameTest : XmlQualifiedName
    {
        private bool exclude;
        private static XmlQualifiedNameTest wc = New("*", "*");
        private const string wildcard = "*";

        private XmlQualifiedNameTest(string name, string ns, bool exclude) : base(name, ns)
        {
            this.exclude = exclude;
        }

        public bool HasIntersection(XmlQualifiedNameTest other)
        {
            if (!this.IsNamespaceSubsetOf(other) && !other.IsNamespaceSubsetOf(this))
            {
                return false;
            }
            if (!this.IsNameSubsetOf(other))
            {
                return other.IsNameSubsetOf(this);
            }
            return true;
        }

        private bool IsNamespaceSubsetOf(XmlQualifiedNameTest other) => 
            ((other.IsNamespaceWildcard || ((this.exclude == other.exclude) && (base.Namespace == other.Namespace))) || ((other.exclude && !this.exclude) && (base.Namespace != other.Namespace)));

        private bool IsNameSubsetOf(XmlQualifiedNameTest other)
        {
            if (!other.IsNameWildcard)
            {
                return (base.Name == other.Name);
            }
            return true;
        }

        public bool IsSubsetOf(XmlQualifiedNameTest other) => 
            (this.IsNameSubsetOf(other) && this.IsNamespaceSubsetOf(other));

        public static XmlQualifiedNameTest New(string name, string ns)
        {
            if ((ns == null) && (name == null))
            {
                return Wildcard;
            }
            return new XmlQualifiedNameTest((name == null) ? "*" : name, (ns == null) ? "*" : ns, false);
        }

        public override string ToString()
        {
            if (this == Wildcard)
            {
                return "*";
            }
            if (base.Namespace.Length == 0)
            {
                return base.Name;
            }
            if (base.Namespace == "*")
            {
                return ("*:" + base.Name);
            }
            if (this.exclude)
            {
                return ("{~" + base.Namespace + "}:" + base.Name);
            }
            return ("{" + base.Namespace + "}:" + base.Name);
        }

        public bool IsNamespaceWildcard =>
            (base.Namespace == "*");

        public bool IsNameWildcard =>
            (base.Name == "*");

        public bool IsWildcard =>
            (this == Wildcard);

        public static XmlQualifiedNameTest Wildcard =>
            wc;
    }
}

