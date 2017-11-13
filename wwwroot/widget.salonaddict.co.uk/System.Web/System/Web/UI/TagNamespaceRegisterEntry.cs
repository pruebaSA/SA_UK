﻿namespace System.Web.UI
{
    using System;

    internal class TagNamespaceRegisterEntry : RegisterDirectiveEntry
    {
        private string _assemblyName;
        private string _ns;

        internal TagNamespaceRegisterEntry(string tagPrefix, string namespaceName, string assemblyName) : base(tagPrefix)
        {
            this._ns = namespaceName;
            this._assemblyName = assemblyName;
        }

        internal string AssemblyName =>
            this._assemblyName;

        internal string Namespace =>
            this._ns;
    }
}

