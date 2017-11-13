namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Xml;

    internal class InputScope : DocumentScope
    {
        private bool canHaveApplyImports;
        private Hashtable excludedNamespaces;
        private Hashtable extensionNamespaces;
        private bool forwardCompatibility;
        private InputScope parent;
        private Hashtable variables;

        internal InputScope(InputScope parent)
        {
            this.Init(parent);
        }

        internal int GetVeriablesCount() => 
            this.variables?.Count;

        internal void Init(InputScope parent)
        {
            base.scopes = null;
            this.parent = parent;
            if (this.parent != null)
            {
                this.forwardCompatibility = this.parent.forwardCompatibility;
                this.canHaveApplyImports = this.parent.canHaveApplyImports;
            }
        }

        internal void InsertExcludedNamespace(string nspace)
        {
            if (this.excludedNamespaces == null)
            {
                this.excludedNamespaces = new Hashtable();
            }
            this.excludedNamespaces[nspace] = null;
        }

        internal void InsertExtensionNamespace(string nspace)
        {
            if (this.extensionNamespaces == null)
            {
                this.extensionNamespaces = new Hashtable();
            }
            this.extensionNamespaces[nspace] = null;
        }

        internal void InsertVariable(VariableAction variable)
        {
            if (this.variables == null)
            {
                this.variables = new Hashtable();
            }
            this.variables[variable.Name] = variable;
        }

        internal bool IsExcludedNamespace(string nspace) => 
            this.excludedNamespaces?.Contains(nspace);

        internal bool IsExtensionNamespace(string nspace) => 
            this.extensionNamespaces?.Contains(nspace);

        public VariableAction ResolveGlobalVariable(XmlQualifiedName qname)
        {
            InputScope scope = null;
            for (InputScope scope2 = this; scope2 != null; scope2 = scope2.Parent)
            {
                scope = scope2;
            }
            return scope.ResolveVariable(qname);
        }

        public VariableAction ResolveVariable(XmlQualifiedName qname)
        {
            for (InputScope scope = this; scope != null; scope = scope.Parent)
            {
                if (scope.Variables != null)
                {
                    VariableAction action = (VariableAction) scope.Variables[qname];
                    if (action != null)
                    {
                        return action;
                    }
                }
            }
            return null;
        }

        internal bool CanHaveApplyImports
        {
            get => 
                this.canHaveApplyImports;
            set
            {
                this.canHaveApplyImports = value;
            }
        }

        internal bool ForwardCompatibility
        {
            get => 
                this.forwardCompatibility;
            set
            {
                this.forwardCompatibility = value;
            }
        }

        internal InputScope Parent =>
            this.parent;

        internal Hashtable Variables =>
            this.variables;
    }
}

