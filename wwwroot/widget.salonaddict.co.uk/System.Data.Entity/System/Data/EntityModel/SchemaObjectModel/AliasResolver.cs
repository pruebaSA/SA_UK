namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal sealed class AliasResolver
    {
        private Dictionary<string, string> _aliasToNamespaceMap = new Dictionary<string, string>(StringComparer.Ordinal);
        private Schema _definingSchema;
        private List<UsingElement> _usingElementCollection = new List<UsingElement>();

        public AliasResolver(Schema schema)
        {
            this._definingSchema = schema;
            if (!string.IsNullOrEmpty(schema.Alias))
            {
                this._aliasToNamespaceMap.Add(schema.Alias, schema.Namespace);
            }
        }

        public void Add(UsingElement usingElement)
        {
            string namespaceName = usingElement.NamespaceName;
            string alias = usingElement.Alias;
            if (this.CheckForSystemNamespace(usingElement, alias, NameKind.Alias))
            {
                alias = null;
            }
            if (this.CheckForSystemNamespace(usingElement, namespaceName, NameKind.Namespace))
            {
                namespaceName = null;
            }
            if ((alias != null) && this._aliasToNamespaceMap.ContainsKey(alias))
            {
                usingElement.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, Strings.AliasNameIsAlreadyDefined(alias));
                alias = null;
            }
            if (alias != null)
            {
                this._aliasToNamespaceMap.Add(alias, namespaceName);
                this._usingElementCollection.Add(usingElement);
            }
        }

        private bool CheckForSystemNamespace(UsingElement refSchema, string name, NameKind nameKind)
        {
            if (!EdmItemCollection.IsSystemNamespace(this._definingSchema.ProviderManifest, name))
            {
                return false;
            }
            if (nameKind == NameKind.Alias)
            {
                refSchema.AddError(ErrorCode.CannotUseSystemNamespaceAsAlias, EdmSchemaErrorSeverity.Error, Strings.CannotUseSystemNamespaceAsAlias(name));
            }
            else
            {
                refSchema.AddError(ErrorCode.NeedNotUseSystemNamespaceInUsing, EdmSchemaErrorSeverity.Error, Strings.NeedNotUseSystemNamespaceInUsing(name));
            }
            return true;
        }

        public void ResolveNamespaces()
        {
            foreach (UsingElement element in this._usingElementCollection)
            {
                if (!this._definingSchema.SchemaManager.IsValidNamespaceName(element.NamespaceName))
                {
                    element.AddError(ErrorCode.InvalidNamespaceInUsing, EdmSchemaErrorSeverity.Error, Strings.InvalidNamespaceInUsing(element.NamespaceName));
                }
            }
        }

        public bool TryResolveAlias(string alias, out string namespaceName) => 
            this._aliasToNamespaceMap.TryGetValue(alias, out namespaceName);

        private enum NameKind
        {
            Alias,
            Namespace
        }
    }
}

