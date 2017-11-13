namespace System.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Entity;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;
    using System.Xml;

    public abstract class DbXmlEnabledProviderManifest : DbProviderManifest
    {
        private Dictionary<PrimitiveType, System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription>> _facetDescriptions = new Dictionary<PrimitiveType, System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription>>();
        private System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> _functions;
        private string _namespaceName;
        private System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> _primitiveTypes;
        private Dictionary<string, PrimitiveType> _storeTypeNameToEdmPrimitiveType = new Dictionary<string, PrimitiveType>();
        private Dictionary<string, PrimitiveType> _storeTypeNameToStorePrimitiveType = new Dictionary<string, PrimitiveType>();

        protected DbXmlEnabledProviderManifest(XmlReader reader)
        {
            if (reader == null)
            {
                throw EntityUtil.ProviderIncompatible(Strings.IncorrectProviderManifest, new ArgumentNullException("reader"));
            }
            this.Load(reader);
        }

        private static bool EnumerableToReadOnlyCollection<Target, BaseType>(IEnumerable<BaseType> enumerable, out System.Collections.ObjectModel.ReadOnlyCollection<Target> collection) where Target: BaseType
        {
            List<Target> list = new List<Target>();
            foreach (BaseType local in enumerable)
            {
                if ((typeof(Target) == typeof(BaseType)) || (local is Target))
                {
                    list.Add((Target) local);
                }
            }
            if (list.Count != 0)
            {
                collection = list.AsReadOnly();
                return true;
            }
            collection = null;
            return false;
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription> GetFacetDescriptions(EdmType type) => 
            GetReadOnlyCollection<FacetDescription>(type as PrimitiveType, this._facetDescriptions, Helper.EmptyFacetDescriptionEnumerable);

        private static System.Collections.ObjectModel.ReadOnlyCollection<T> GetReadOnlyCollection<T>(PrimitiveType type, Dictionary<PrimitiveType, System.Collections.ObjectModel.ReadOnlyCollection<T>> typeDictionary, System.Collections.ObjectModel.ReadOnlyCollection<T> useIfEmpty)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<T> onlys;
            if (typeDictionary.TryGetValue(type, out onlys))
            {
                return onlys;
            }
            return useIfEmpty;
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> GetStoreFunctions() => 
            this._functions;

        public override System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetStoreTypes() => 
            this._primitiveTypes;

        private void Load(XmlReader reader)
        {
            Schema schema;
            IList<EdmSchemaError> errors = SchemaManager.LoadProviderManifest(reader, (reader.BaseURI.Length > 0) ? reader.BaseURI : null, true, out schema);
            if (errors.Count != 0)
            {
                throw EntityUtil.ProviderIncompatible(Strings.IncorrectProviderManifest + Helper.CombineErrorMessage(errors));
            }
            this._namespaceName = schema.Namespace;
            List<PrimitiveType> list2 = new List<PrimitiveType>();
            foreach (System.Data.EntityModel.SchemaObjectModel.SchemaType type in schema.SchemaTypes)
            {
                TypeElement element = type as TypeElement;
                if (element != null)
                {
                    System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription> onlys;
                    PrimitiveType primitiveType = element.PrimitiveType;
                    primitiveType.ProviderManifest = this;
                    primitiveType.DataSpace = DataSpace.SSpace;
                    primitiveType.SetReadOnly();
                    list2.Add(primitiveType);
                    this._storeTypeNameToStorePrimitiveType.Add(primitiveType.Name.ToLowerInvariant(), primitiveType);
                    this._storeTypeNameToEdmPrimitiveType.Add(primitiveType.Name.ToLowerInvariant(), EdmProviderManifest.Instance.GetPrimitiveType(primitiveType.PrimitiveTypeKind));
                    if (EnumerableToReadOnlyCollection<FacetDescription, FacetDescription>(element.FacetDescriptions, out onlys))
                    {
                        this._facetDescriptions.Add(primitiveType, onlys);
                    }
                }
            }
            this._primitiveTypes = Array.AsReadOnly<PrimitiveType>(list2.ToArray());
            ItemCollection itemCollection = new EmptyItemCollection();
            if (!EnumerableToReadOnlyCollection<EdmFunction, GlobalItem>(Converter.ConvertSchema(schema, this, itemCollection), out this._functions))
            {
                this._functions = Helper.EmptyEdmFunctionReadOnlyCollection;
            }
            foreach (EdmFunction function in this._functions)
            {
                function.SetReadOnly();
            }
        }

        public override string NamespaceName =>
            this._namespaceName;

        protected Dictionary<string, PrimitiveType> StoreTypeNameToEdmPrimitiveType =>
            this._storeTypeNameToEdmPrimitiveType;

        protected Dictionary<string, PrimitiveType> StoreTypeNameToStorePrimitiveType =>
            this._storeTypeNameToStorePrimitiveType;

        private class EmptyItemCollection : ItemCollection
        {
            public EmptyItemCollection() : base(DataSpace.SSpace)
            {
            }
        }
    }
}

