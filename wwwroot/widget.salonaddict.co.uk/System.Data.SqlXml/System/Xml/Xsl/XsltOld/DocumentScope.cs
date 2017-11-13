namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal class DocumentScope
    {
        protected NamespaceDecl scopes;

        internal NamespaceDecl AddNamespace(string prefix, string uri, string prevDefaultNsUri)
        {
            this.scopes = new NamespaceDecl(prefix, uri, prevDefaultNsUri, this.scopes);
            return this.scopes;
        }

        internal string ResolveAtom(string prefix)
        {
            for (NamespaceDecl decl = this.scopes; decl != null; decl = decl.Next)
            {
                if (Keywords.Equals(decl.Prefix, prefix))
                {
                    return decl.Uri;
                }
            }
            return null;
        }

        internal string ResolveNonAtom(string prefix)
        {
            for (NamespaceDecl decl = this.scopes; decl != null; decl = decl.Next)
            {
                if (Keywords.Compare(decl.Prefix, prefix))
                {
                    return decl.Uri;
                }
            }
            return null;
        }

        internal NamespaceDecl Scopes =>
            this.scopes;
    }
}

