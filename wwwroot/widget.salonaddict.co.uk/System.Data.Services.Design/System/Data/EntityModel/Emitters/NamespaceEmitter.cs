namespace System.Data.EntityModel.Emitters
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Design;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal sealed class NamespaceEmitter : Emitter
    {
        private string _namespacePrefix;
        private string _targetFilePath;
        private static Pair<Type, CreateEmitter>[] EmitterCreators = new Pair<Type, CreateEmitter>[] { new Pair<Type, CreateEmitter>(typeof(EntityType), (CreateEmitter) ((generator1, element) => new EntityTypeEmitter(generator1, (EntityType) element))), new Pair<Type, CreateEmitter>(typeof(ComplexType), (CreateEmitter) ((generator2, element) => new ComplexTypeEmitter(generator2, (ComplexType) element))), new Pair<Type, CreateEmitter>(typeof(EntityContainer), (CreateEmitter) ((generator3, element) => new EntityContainerEmitter(generator3, (EntityContainer) element))), new Pair<Type, CreateEmitter>(typeof(AssociationType), (CreateEmitter) ((generator4, element) => new AssociationTypeEmitter(generator4, (AssociationType) element))) };

        public NamespaceEmitter(ClientApiGenerator generator, string namespacePrefix, string targetFilePath) : base(generator)
        {
            this._targetFilePath = (targetFilePath != null) ? targetFilePath : string.Empty;
            this._namespacePrefix = namespacePrefix;
        }

        [CompilerGenerated]
        private static SchemaTypeEmitter <.cctor>b__9(ClientApiGenerator generator1, GlobalItem element) => 
            new EntityTypeEmitter(generator1, (EntityType) element);

        [CompilerGenerated]
        private static SchemaTypeEmitter <.cctor>b__a(ClientApiGenerator generator2, GlobalItem element) => 
            new ComplexTypeEmitter(generator2, (ComplexType) element);

        [CompilerGenerated]
        private static SchemaTypeEmitter <.cctor>b__b(ClientApiGenerator generator3, GlobalItem element) => 
            new EntityContainerEmitter(generator3, (EntityContainer) element);

        [CompilerGenerated]
        private static SchemaTypeEmitter <.cctor>b__c(ClientApiGenerator generator4, GlobalItem element) => 
            new AssociationTypeEmitter(generator4, (AssociationType) element);

        private bool AddElementNameToCache(GlobalItem element, Dictionary<string, string> cache)
        {
            if (element.BuiltInTypeKind == BuiltInTypeKind.EntityContainer)
            {
                return this.TryAddNameToCache((element as EntityContainer).Name, element.BuiltInTypeKind.ToString(), cache);
            }
            return ((((element.BuiltInTypeKind != BuiltInTypeKind.EntityType) && (element.BuiltInTypeKind != BuiltInTypeKind.ComplexType)) && (element.BuiltInTypeKind != BuiltInTypeKind.AssociationType)) || this.TryAddNameToCache((element as StructuralType).Name, element.BuiltInTypeKind.ToString(), cache));
        }

        private void BuildNamespaceMap(string defaultContainerNamespace)
        {
            var selector = null;
            if (!string.IsNullOrEmpty(base.Generator.NamespacePrefix))
            {
                if (selector == null)
                {
                    selector = ns => new { 
                        ServiceNamespace = ns,
                        ClientNamespace = (ns == defaultContainerNamespace) ? this._namespacePrefix : (this._namespacePrefix + "." + ns)
                    };
                }
                foreach (var type in (from et in base.Generator.GetSourceTypes().OfType<EdmType>() select et.NamespaceName).Distinct<string>().Select(selector))
                {
                    base.Generator.NamespaceMap.Add(type.ServiceNamespace, type.ClientNamespace);
                }
            }
        }

        private SchemaTypeEmitter CreateElementEmitter(GlobalItem element)
        {
            Type c = element.GetType();
            foreach (Pair<Type, CreateEmitter> pair in EmitterCreators)
            {
                if (pair.First.IsAssignableFrom(c))
                {
                    return pair.Second(base.Generator, element);
                }
            }
            return null;
        }

        public void Emit()
        {
            Dictionary<string, string> usedClassName = new Dictionary<string, string>(StringComparer.Ordinal);
            HashSet<string> set = new HashSet<string> { "Edm" };
            EntityContainer container = base.Generator.EdmItemCollection.GetItems<EntityContainer>().FirstOrDefault<EntityContainer>(c => this.IsDefaultContainer(c));
            base.Generator.DefaultContainerNamespace = base.Generator.GetContainerNamespace(container);
            this.BuildNamespaceMap(base.Generator.DefaultContainerNamespace);
            foreach (EntityContainer container2 in base.Generator.EdmItemCollection.GetItems<EntityContainer>())
            {
                string containerNamespace = base.Generator.GetContainerNamespace(container2);
                set.Add(containerNamespace);
                List<GlobalItem> items = new List<GlobalItem> {
                    container2
                };
                foreach (GlobalItem item in base.Generator.GetSourceTypes())
                {
                    EdmType type = item as EdmType;
                    if ((type != null) && (type.NamespaceName == containerNamespace))
                    {
                        items.Add(type);
                    }
                }
                if (!string.IsNullOrEmpty(this._namespacePrefix))
                {
                    if (string.IsNullOrEmpty(containerNamespace) || this.IsDefaultContainer(container2))
                    {
                        containerNamespace = this._namespacePrefix;
                    }
                    else
                    {
                        containerNamespace = this._namespacePrefix + "." + containerNamespace;
                    }
                }
                this.Emit(usedClassName, containerNamespace, items);
            }
            foreach (string str2 in (from x in base.Generator.EdmItemCollection.GetItems<EdmType>() select x.NamespaceName).Distinct<string>())
            {
                if (set.Add(str2))
                {
                    List<GlobalItem> list2 = new List<GlobalItem>();
                    foreach (GlobalItem item2 in base.Generator.GetSourceTypes())
                    {
                        EdmType type2 = item2 as EdmType;
                        if ((type2 != null) && (type2.NamespaceName == str2))
                        {
                            list2.Add(type2);
                        }
                    }
                    if (0 < list2.Count)
                    {
                        string namespaceName = str2;
                        if (this._namespacePrefix != null)
                        {
                            namespaceName = base.Generator.GetClientTypeNamespace(str2);
                        }
                        this.Emit(usedClassName, namespaceName, list2);
                    }
                }
            }
        }

        private void Emit(Dictionary<string, string> usedClassName, string namespaceName, List<GlobalItem> items)
        {
            base.Generator.SourceEdmNamespaceName = namespaceName;
            CodeNamespace namespace2 = new CodeNamespace(namespaceName);
            CommentEmitter.EmitComments(CommentEmitter.GetFormattedLines(System.Data.Services.Design.Strings.NamespaceComments(Path.GetFileName(this._targetFilePath), DateTime.Now.ToString(CultureInfo.CurrentCulture)), false), namespace2.Comments, false);
            this.CompileUnit.Namespaces.Add(namespace2);
            foreach (GlobalItem item in items)
            {
                if (this.AddElementNameToCache(item, usedClassName))
                {
                    CodeTypeDeclarationCollection declarations = this.CreateElementEmitter(item).EmitApiClass();
                    if (declarations.Count > 0)
                    {
                        namespace2.Types.AddRange(declarations);
                    }
                }
            }
            base.Generator.SourceEdmNamespaceName = null;
        }

        private bool IsDefaultContainer(EntityContainer container)
        {
            MetadataProperty property;
            if (container == null)
            {
                return false;
            }
            if (!container.MetadataProperties.TryGetValue("http://schemas.microsoft.com/ado/2007/08/dataservices/metadata:IsDefaultEntityContainer", false, out property))
            {
                return false;
            }
            return (((property != null) && (property.Value != null)) && string.Equals(property.Value.ToString(), "true", StringComparison.OrdinalIgnoreCase));
        }

        private bool TryAddNameToCache(string name, string type, Dictionary<string, string> cache)
        {
            if (!cache.ContainsKey(name))
            {
                cache.Add(name, type);
                return true;
            }
            base.Generator.AddError(System.Data.Services.Design.Strings.DuplicateClassName(type, name, cache[name]), ModelBuilderErrorCode.DuplicateClassName, EdmSchemaErrorSeverity.Error);
            return false;
        }

        private CodeCompileUnit CompileUnit =>
            base.Generator.CompileUnit;

        private delegate SchemaTypeEmitter CreateEmitter(ClientApiGenerator generator, GlobalItem item);

        private class Pair<T1, T2>
        {
            public T1 First;
            public T2 Second;

            internal Pair(T1 first, T2 second)
            {
                this.First = first;
                this.Second = second;
            }
        }
    }
}

