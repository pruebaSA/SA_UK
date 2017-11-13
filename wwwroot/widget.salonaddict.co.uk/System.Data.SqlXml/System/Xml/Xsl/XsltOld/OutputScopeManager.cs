namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class OutputScopeManager
    {
        private OutKeywords atoms;
        private string defaultNS;
        private HWStack elementScopesStack = new HWStack(10);
        private XmlNameTable nameTable;
        private int prefixIndex;
        private const int STACK_INCREMENT = 10;

        internal OutputScopeManager(XmlNameTable nameTable, OutKeywords atoms)
        {
            this.nameTable = nameTable;
            this.atoms = atoms;
            this.defaultNS = this.atoms.Empty;
            OutputScope o = (OutputScope) this.elementScopesStack.Push();
            if (o == null)
            {
                o = new OutputScope();
                this.elementScopesStack.AddToTop(o);
            }
            o.Init(string.Empty, string.Empty, string.Empty, System.Xml.XmlSpace.None, string.Empty, false);
        }

        internal bool FindPrefix(string nspace, out string prefix)
        {
            for (int i = this.elementScopesStack.Length - 1; 0 <= i; i--)
            {
                OutputScope scope = (OutputScope) this.elementScopesStack[i];
                string str = null;
                if (scope.FindPrefix(nspace, out str))
                {
                    string strA = this.ResolveNamespace(str);
                    if ((strA != null) && Keywords.Equals(strA, nspace))
                    {
                        prefix = str;
                        return true;
                    }
                    break;
                }
            }
            prefix = null;
            return false;
        }

        internal string GeneratePrefix(string format)
        {
            string str;
            do
            {
                str = string.Format(CultureInfo.InvariantCulture, format, new object[] { this.prefixIndex++ });
            }
            while (this.nameTable.Get(str) != null);
            return this.nameTable.Add(str);
        }

        internal void PopScope()
        {
            OutputScope scope = (OutputScope) this.elementScopesStack.Pop();
            for (System.Xml.Xsl.XsltOld.NamespaceDecl decl = scope.Scopes; decl != null; decl = decl.Next)
            {
                this.defaultNS = decl.PrevDefaultNsUri;
            }
        }

        internal void PushNamespace(string prefix, string nspace)
        {
            this.CurrentElementScope.AddNamespace(prefix, nspace, this.defaultNS);
            if ((prefix == null) || (prefix.Length == 0))
            {
                this.defaultNS = nspace;
            }
        }

        internal void PushScope(string name, string nspace, string prefix)
        {
            OutputScope currentElementScope = this.CurrentElementScope;
            OutputScope o = (OutputScope) this.elementScopesStack.Push();
            if (o == null)
            {
                o = new OutputScope();
                this.elementScopesStack.AddToTop(o);
            }
            o.Init(name, nspace, prefix, currentElementScope.Space, currentElementScope.Lang, currentElementScope.Mixed);
        }

        internal string ResolveNamespace(string prefix)
        {
            bool flag;
            return this.ResolveNamespace(prefix, out flag);
        }

        internal string ResolveNamespace(string prefix, out bool thisScope)
        {
            thisScope = true;
            if ((prefix == null) || (prefix.Length == 0))
            {
                return this.defaultNS;
            }
            if (Keywords.Equals(prefix, this.atoms.Xml))
            {
                return this.atoms.XmlNamespace;
            }
            if (Keywords.Equals(prefix, this.atoms.Xmlns))
            {
                return this.atoms.XmlnsNamespace;
            }
            for (int i = this.elementScopesStack.Length - 1; i >= 0; i--)
            {
                string str = ((OutputScope) this.elementScopesStack[i]).ResolveAtom(prefix);
                if (str != null)
                {
                    thisScope = i == (this.elementScopesStack.Length - 1);
                    return str;
                }
            }
            return null;
        }

        internal OutputScope CurrentElementScope =>
            ((OutputScope) this.elementScopesStack.Peek());

        internal string DefaultNamespace =>
            this.defaultNS;

        internal string XmlLang =>
            this.CurrentElementScope.Lang;

        internal System.Xml.XmlSpace XmlSpace =>
            this.CurrentElementScope.Space;
    }
}

