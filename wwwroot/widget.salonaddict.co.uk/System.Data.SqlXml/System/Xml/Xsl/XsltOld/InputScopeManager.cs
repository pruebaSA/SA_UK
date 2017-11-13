namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class InputScopeManager
    {
        private string defaultNS = string.Empty;
        private XPathNavigator navigator;
        private InputScope scopeStack;

        public InputScopeManager(XPathNavigator navigator, InputScope rootScope)
        {
            this.navigator = navigator;
            this.scopeStack = rootScope;
        }

        internal InputScopeManager Clone() => 
            new InputScopeManager(this.navigator, null) { 
                scopeStack = this.scopeStack,
                defaultNS = this.defaultNS
            };

        internal void InsertExcludedNamespaces(string[] nsList)
        {
            for (int i = 0; i < nsList.Length; i++)
            {
                this.scopeStack.InsertExcludedNamespace(nsList[i]);
            }
        }

        internal void InsertExtensionNamespaces(string[] nsList)
        {
            for (int i = 0; i < nsList.Length; i++)
            {
                this.scopeStack.InsertExtensionNamespace(nsList[i]);
            }
        }

        internal bool IsExcludedNamespace(string nspace)
        {
            for (InputScope scope = this.scopeStack; scope != null; scope = scope.Parent)
            {
                if (scope.IsExcludedNamespace(nspace))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsExtensionNamespace(string nspace)
        {
            for (InputScope scope = this.scopeStack; scope != null; scope = scope.Parent)
            {
                if (scope.IsExtensionNamespace(nspace))
                {
                    return true;
                }
            }
            return false;
        }

        internal void PopScope()
        {
            if (this.scopeStack != null)
            {
                for (NamespaceDecl decl = this.scopeStack.Scopes; decl != null; decl = decl.Next)
                {
                    this.defaultNS = decl.PrevDefaultNsUri;
                }
                this.scopeStack = this.scopeStack.Parent;
            }
        }

        internal void PushNamespace(string prefix, string nspace)
        {
            this.scopeStack.AddNamespace(prefix, nspace, this.defaultNS);
            if ((prefix == null) || (prefix.Length == 0))
            {
                this.defaultNS = nspace;
            }
        }

        internal InputScope PushScope()
        {
            this.scopeStack = new InputScope(this.scopeStack);
            return this.scopeStack;
        }

        private string ResolveNonEmptyPrefix(string prefix)
        {
            if (prefix == "xml")
            {
                return "http://www.w3.org/XML/1998/namespace";
            }
            if (prefix == "xmlns")
            {
                return "http://www.w3.org/2000/xmlns/";
            }
            for (InputScope scope = this.scopeStack; scope != null; scope = scope.Parent)
            {
                string str = scope.ResolveNonAtom(prefix);
                if (str != null)
                {
                    return str;
                }
            }
            throw XsltException.Create("Xslt_InvalidPrefix", new string[] { prefix });
        }

        public string ResolveXmlNamespace(string prefix)
        {
            if (prefix.Length == 0)
            {
                return this.defaultNS;
            }
            return this.ResolveNonEmptyPrefix(prefix);
        }

        public string ResolveXPathNamespace(string prefix)
        {
            if (prefix.Length == 0)
            {
                return string.Empty;
            }
            return this.ResolveNonEmptyPrefix(prefix);
        }

        internal InputScope CurrentScope =>
            this.scopeStack;

        public string DefaultNamespace =>
            this.defaultNS;

        public XPathNavigator Navigator =>
            this.navigator;

        internal InputScope VariableScope =>
            this.scopeStack.Parent;
    }
}

