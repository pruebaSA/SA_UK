namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.Data.Objects.ELinq;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class EdmItemCollection : ItemCollection
    {
        private double _edmVersion;
        private Memoizer<InitializerMetadata, InitializerMetadata> _getCanonicalInitializerMetadataMemoizer;
        private CacheForPrimitiveTypes _primitiveTypeMaps;

        public EdmItemCollection(IEnumerable<XmlReader> xmlReaders) : base(DataSpace.CSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._edmVersion = 1.0;
            EntityUtil.CheckArgumentNull<IEnumerable<XmlReader>>(xmlReaders, "xmlReaders");
            MetadataArtifactLoader loader = MetadataArtifactLoader.CreateCompositeFromXmlReaders(xmlReaders);
            this.Init(loader.GetReaders(), loader.GetPaths(), true);
        }

        internal EdmItemCollection(IList<Schema> schemas) : base(DataSpace.CSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._edmVersion = 1.0;
            this.Init();
            LoadItems(MetadataItem.EdmProviderManifest, schemas, this);
        }

        public EdmItemCollection(params string[] filePaths) : base(DataSpace.CSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._edmVersion = 1.0;
            EntityUtil.CheckArgumentNull<string[]>(filePaths, "filePaths");
            MetadataArtifactLoader loader = null;
            List<XmlReader> xmlReaders = null;
            try
            {
                loader = MetadataArtifactLoader.CreateCompositeFromFilePaths(filePaths, ".csdl");
                xmlReaders = loader.CreateReaders(DataSpace.CSpace);
                this.Init(xmlReaders, loader.GetPaths(DataSpace.CSpace), true);
            }
            finally
            {
                if (xmlReaders != null)
                {
                    Helper.DisposeXmlReaders(xmlReaders);
                }
            }
        }

        internal EdmItemCollection(IEnumerable<XmlReader> xmlReaders, IEnumerable<string> filePaths) : base(DataSpace.CSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._edmVersion = 1.0;
            this.Init(xmlReaders, filePaths, true);
        }

        internal EdmItemCollection(IEnumerable<XmlReader> xmlReaders, System.Collections.ObjectModel.ReadOnlyCollection<string> filePaths, out IList<EdmSchemaError> errors) : base(DataSpace.CSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._edmVersion = 1.0;
            errors = this.Init(xmlReaders, filePaths, false);
        }

        internal InitializerMetadata GetCanonicalInitializerMetadata(InitializerMetadata metadata) => 
            this._getCanonicalInitializerMetadataMemoizer?.Evaluate(metadata);

        internal override PrimitiveType GetMappedPrimitiveType(PrimitiveTypeKind primitiveTypeKind)
        {
            PrimitiveType type = null;
            this._primitiveTypeMaps.TryGetType(primitiveTypeKind, null, out type);
            return type;
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetPrimitiveTypes() => 
            this._primitiveTypeMaps.GetTypes();

        private void Init()
        {
            this.LoadEdmPrimitiveTypesAndFunctions();
        }

        private IList<EdmSchemaError> Init(IEnumerable<XmlReader> xmlReaders, IEnumerable<string> filePaths, bool throwOnError)
        {
            EntityUtil.CheckArgumentNull<IEnumerable<XmlReader>>(xmlReaders, "xmlReaders");
            this.Init();
            return LoadItems(xmlReaders, filePaths, SchemaDataModelOption.EntityDataModel, MetadataItem.EdmProviderManifest, this, throwOnError);
        }

        internal static bool IsSystemNamespace(DbProviderManifest manifest, string namespaceName)
        {
            if (manifest == MetadataItem.EdmProviderManifest)
            {
                if ((namespaceName != "Transient") && (namespaceName != "Edm"))
                {
                    return (namespaceName == "System");
                }
                return true;
            }
            return ((((namespaceName == "Transient") || (namespaceName == "Edm")) || (namespaceName == "System")) || ((manifest != null) && (namespaceName == manifest.NamespaceName)));
        }

        private void LoadEdmPrimitiveTypesAndFunctions()
        {
            EdmProviderManifest instance = EdmProviderManifest.Instance;
            System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> storeTypes = instance.GetStoreTypes();
            for (int i = 0; i < storeTypes.Count; i++)
            {
                base.AddInternal(storeTypes[i]);
                this._primitiveTypeMaps.Add(storeTypes[i]);
            }
            System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> storeFunctions = instance.GetStoreFunctions();
            for (int j = 0; j < storeFunctions.Count; j++)
            {
                base.AddInternal(storeFunctions[j]);
            }
        }

        internal static List<EdmSchemaError> LoadItems(DbProviderManifest manifest, IList<Schema> somSchemas, ItemCollection itemCollection)
        {
            List<EdmSchemaError> list = new List<EdmSchemaError>();
            IEnumerable<GlobalItem> enumerable = LoadSomSchema(somSchemas, manifest, itemCollection);
            List<string> list2 = new List<string>();
            foreach (GlobalItem item in enumerable)
            {
                if ((item.BuiltInTypeKind == BuiltInTypeKind.EdmFunction) && (item.DataSpace == DataSpace.SSpace))
                {
                    EdmFunction function = (EdmFunction) item;
                    StringBuilder builder = new StringBuilder();
                    builder.Append(function.FullName);
                    foreach (FunctionParameter parameter in function.Parameters)
                    {
                        TypeUsage usage = MetadataHelper.ConvertStoreTypeUsageToEdmTypeUsage(parameter.TypeUsage);
                        builder.Append(" ").Append(usage.Identity).Append(" ");
                    }
                    if (list2.Contains(builder.ToString()))
                    {
                        list.Add(new EdmSchemaError(Strings.DuplicatedFunctionoverloads(function.FullName, builder.ToString().Substring(function.FullName.Length)), 0xae, EdmSchemaErrorSeverity.Error));
                        continue;
                    }
                    list2.Add(builder.ToString());
                }
                item.SetReadOnly();
                itemCollection.AddInternal(item);
            }
            return list;
        }

        internal static IList<EdmSchemaError> LoadItems(IEnumerable<XmlReader> xmlReaders, IEnumerable<string> sourceFilePaths, SchemaDataModelOption dataModelOption, DbProviderManifest providerManifest, ItemCollection itemCollection, bool throwOnError)
        {
            IList<Schema> schemaCollection = null;
            IList<EdmSchemaError> schemaErrors = SchemaManager.ParseAndValidate(xmlReaders, sourceFilePaths, dataModelOption, providerManifest, out schemaCollection);
            if (MetadataHelper.CheckIfAllErrorsAreWarnings(schemaErrors))
            {
                foreach (EdmSchemaError error in LoadItems(providerManifest, schemaCollection, itemCollection))
                {
                    schemaErrors.Add(error);
                }
            }
            if (!MetadataHelper.CheckIfAllErrorsAreWarnings(schemaErrors) && throwOnError)
            {
                throw EntityUtil.InvalidSchemaEncountered(Helper.CombineErrorMessage(schemaErrors));
            }
            return schemaErrors;
        }

        internal static IEnumerable<GlobalItem> LoadSomSchema(IList<Schema> somSchemas, DbProviderManifest providerManifest, ItemCollection itemCollection) => 
            Converter.ConvertSchema(somSchemas, providerManifest, itemCollection);

        public double EdmVersion
        {
            get => 
                this._edmVersion;
            internal set
            {
                this._edmVersion = value;
            }
        }
    }
}

