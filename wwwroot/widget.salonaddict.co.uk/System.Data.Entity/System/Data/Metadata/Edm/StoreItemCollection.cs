namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.QueryCache;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public sealed class StoreItemCollection : ItemCollection
    {
        private readonly Memoizer<EdmFunction, EdmFunction> _cachedCTypeFunction;
        private readonly CacheForPrimitiveTypes _primitiveTypeMaps;
        private readonly DbProviderFactory _providerFactory;
        private readonly DbProviderManifest _providerManifest;
        private readonly System.Data.Common.QueryCache.QueryCacheManager _queryCacheManager;

        public StoreItemCollection(IEnumerable<XmlReader> xmlReaders) : base(DataSpace.SSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._queryCacheManager = System.Data.Common.QueryCache.QueryCacheManager.Create();
            EntityUtil.CheckArgumentNull<IEnumerable<XmlReader>>(xmlReaders, "xmlReaders");
            EntityUtil.CheckArgumentEmpty<XmlReader>(ref xmlReaders, new Func<string, string>(System.Data.Entity.Strings.StoreItemCollectionMustHaveOneArtifact), "xmlReader");
            MetadataArtifactLoader loader = MetadataArtifactLoader.CreateCompositeFromXmlReaders(xmlReaders);
            this.Init(loader.GetReaders(), loader.GetPaths(), true, out this._providerManifest, out this._providerFactory, out this._cachedCTypeFunction);
        }

        public StoreItemCollection(params string[] filePaths) : base(DataSpace.SSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._queryCacheManager = System.Data.Common.QueryCache.QueryCacheManager.Create();
            EntityUtil.CheckArgumentNull<string[]>(filePaths, "filePaths");
            IEnumerable<string> enumerableArgument = filePaths;
            EntityUtil.CheckArgumentEmpty<string>(ref enumerableArgument, new Func<string, string>(System.Data.Entity.Strings.StoreItemCollectionMustHaveOneArtifact), "filePaths");
            MetadataArtifactLoader loader = null;
            List<XmlReader> source = null;
            try
            {
                loader = MetadataArtifactLoader.CreateCompositeFromFilePaths(enumerableArgument, ".ssdl");
                source = loader.CreateReaders(DataSpace.SSpace);
                EntityUtil.CheckArgumentEmpty<XmlReader>(ref source.AsEnumerable<XmlReader>(), new Func<string, string>(System.Data.Entity.Strings.StoreItemCollectionMustHaveOneArtifact), "filePaths");
                this.Init(source, loader.GetPaths(DataSpace.SSpace), true, out this._providerManifest, out this._providerFactory, out this._cachedCTypeFunction);
            }
            finally
            {
                if (source != null)
                {
                    Helper.DisposeXmlReaders(source);
                }
            }
        }

        internal StoreItemCollection(IEnumerable<XmlReader> xmlReaders, IEnumerable<string> filePaths) : base(DataSpace.SSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._queryCacheManager = System.Data.Common.QueryCache.QueryCacheManager.Create();
            EntityUtil.CheckArgumentNull<IEnumerable<string>>(filePaths, "filePaths");
            EntityUtil.CheckArgumentEmpty<XmlReader>(ref xmlReaders, new Func<string, string>(System.Data.Entity.Strings.StoreItemCollectionMustHaveOneArtifact), "xmlReader");
            this.Init(xmlReaders, filePaths, true, out this._providerManifest, out this._providerFactory, out this._cachedCTypeFunction);
        }

        internal StoreItemCollection(DbProviderFactory factory, DbProviderManifest manifest) : base(DataSpace.SSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._queryCacheManager = System.Data.Common.QueryCache.QueryCacheManager.Create();
            this._providerFactory = factory;
            this._providerManifest = manifest;
            this._cachedCTypeFunction = new Memoizer<EdmFunction, EdmFunction>(new Func<EdmFunction, EdmFunction>(this.ConvertFunctionParameterToCType), null);
            this.LoadProviderManifest(this._providerManifest, true);
        }

        internal StoreItemCollection(IEnumerable<XmlReader> xmlReaders, System.Collections.ObjectModel.ReadOnlyCollection<string> filePaths, out IList<EdmSchemaError> errors) : base(DataSpace.SSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._queryCacheManager = System.Data.Common.QueryCache.QueryCacheManager.Create();
            EntityUtil.CheckArgumentNull<IEnumerable<XmlReader>>(xmlReaders, "xmlReaders");
            EntityUtil.CheckArgumentEmpty<XmlReader>(ref xmlReaders, new Func<string, string>(System.Data.Entity.Strings.StoreItemCollectionMustHaveOneArtifact), "xmlReader");
            errors = this.Init(xmlReaders, filePaths, false, out this._providerManifest, out this._providerFactory, out this._cachedCTypeFunction);
        }

        private EdmFunction ConvertFunctionParameterToCType(EdmFunction sTypeFunction)
        {
            if (sTypeFunction.IsFromProviderManifest)
            {
                return sTypeFunction;
            }
            FunctionParameter parameter = null;
            if (sTypeFunction.ReturnParameter != null)
            {
                TypeUsage typeUsage = MetadataHelper.ConvertStoreTypeUsageToEdmTypeUsage(sTypeFunction.ReturnParameter.TypeUsage);
                parameter = new FunctionParameter(sTypeFunction.ReturnParameter.Name, typeUsage, sTypeFunction.ReturnParameter.GetParameterMode());
            }
            List<FunctionParameter> list = new List<FunctionParameter>();
            if (sTypeFunction.Parameters.Count > 0)
            {
                foreach (FunctionParameter parameter2 in sTypeFunction.Parameters)
                {
                    TypeUsage usage2 = MetadataHelper.ConvertStoreTypeUsageToEdmTypeUsage(parameter2.TypeUsage);
                    FunctionParameter item = new FunctionParameter(parameter2.Name, usage2, parameter2.GetParameterMode());
                    list.Add(item);
                }
            }
            EdmFunctionPayload payload = new EdmFunctionPayload {
                Schema = sTypeFunction.Schema,
                StoreFunctionName = sTypeFunction.StoreFunctionNameAttribute,
                CommandText = sTypeFunction.CommandTextAttribute,
                IsAggregate = new bool?(sTypeFunction.AggregateAttribute),
                IsBuiltIn = new bool?(sTypeFunction.BuiltInAttribute),
                IsNiladic = new bool?(sTypeFunction.NiladicFunctionAttribute),
                IsComposable = new bool?(sTypeFunction.IsComposableAttribute),
                IsFromProviderManifest = new bool?(sTypeFunction.IsFromProviderManifest),
                IsCachedStoreFunction = true,
                ReturnParameter = parameter,
                Parameters = list.ToArray(),
                ParameterTypeSemantics = new ParameterTypeSemantics?(sTypeFunction.ParameterTypeSemanticsAttribute)
            };
            EdmFunction function = new EdmFunction(sTypeFunction.Name, sTypeFunction.NamespaceName, DataSpace.CSpace, payload);
            function.SetReadOnly();
            return function;
        }

        private System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> ConvertToCTypeFunctions(System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> functionOverloads)
        {
            List<EdmFunction> list = new List<EdmFunction>();
            foreach (EdmFunction function in functionOverloads)
            {
                list.Add(this._cachedCTypeFunction.Evaluate(function));
            }
            return list.AsReadOnly();
        }

        internal System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> GetCTypeFunctions(string functionName, bool ignoreCase)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> onlys;
            if (!base.FunctionLookUpTable.TryGetValue(functionName, out onlys))
            {
                return Helper.EmptyEdmFunctionReadOnlyCollection;
            }
            onlys = this.ConvertToCTypeFunctions(onlys);
            if (ignoreCase)
            {
                return onlys;
            }
            return ItemCollection.GetCaseSensitiveFunctions(onlys, functionName);
        }

        internal override PrimitiveType GetMappedPrimitiveType(PrimitiveTypeKind primitiveTypeKind)
        {
            PrimitiveType type = null;
            this._primitiveTypeMaps.TryGetType(primitiveTypeKind, null, out type);
            return type;
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetPrimitiveTypes() => 
            this._primitiveTypeMaps.GetTypes();

        private IList<EdmSchemaError> Init(IEnumerable<XmlReader> xmlReaders, IEnumerable<string> filePaths, bool throwOnError, out DbProviderManifest providerManifest, out DbProviderFactory providerFactory, out Memoizer<EdmFunction, EdmFunction> cachedCTypeFunction)
        {
            EntityUtil.CheckArgumentNull<IEnumerable<XmlReader>>(xmlReaders, "xmlReaders");
            cachedCTypeFunction = new Memoizer<EdmFunction, EdmFunction>(new Func<EdmFunction, EdmFunction>(this.ConvertFunctionParameterToCType), null);
            Loader loader = new Loader(xmlReaders, filePaths, throwOnError);
            providerFactory = loader.ProviderFactory;
            providerManifest = loader.ProviderManifest;
            if (!loader.HasNonWarningErrors)
            {
                this.LoadProviderManifest(loader.ProviderManifest, true);
                List<EdmSchemaError> list = EdmItemCollection.LoadItems(this._providerManifest, loader.Schemas, this);
                foreach (EdmSchemaError error in list)
                {
                    loader.Errors.Add(error);
                }
                if (throwOnError && (list.Count != 0))
                {
                    loader.ThrowOnNonWarningErrors();
                }
            }
            return loader.Errors;
        }

        private void LoadProviderManifest(DbProviderManifest storeManifest, bool checkForSystemNamespace)
        {
            foreach (PrimitiveType type in storeManifest.GetStoreTypes())
            {
                base.AddInternal(type);
                this._primitiveTypeMaps.Add(type);
            }
            foreach (EdmFunction function in storeManifest.GetStoreFunctions())
            {
                base.AddInternal(function);
            }
        }

        internal System.Data.Common.QueryCache.QueryCacheManager QueryCacheManager =>
            this._queryCacheManager;

        internal DbProviderFactory StoreProviderFactory =>
            this._providerFactory;

        internal DbProviderManifest StoreProviderManifest =>
            this._providerManifest;

        private class Loader
        {
            private IList<EdmSchemaError> _errors;
            private string _provider;
            private DbProviderFactory _providerFactory;
            private DbProviderManifest _providerManifest;
            private string _providerManifestToken;
            private IList<Schema> _schemas;
            private bool _throwOnError;

            public Loader(IEnumerable<XmlReader> xmlReaders, IEnumerable<string> sourceFilePaths, bool throwOnError)
            {
                this._throwOnError = throwOnError;
                this.LoadItems(xmlReaders, sourceFilePaths);
            }

            private void AddProviderIncompatibleError(ProviderIncompatibleException provEx, Action<string, ErrorCode, EdmSchemaErrorSeverity> addError)
            {
                StringBuilder builder = new StringBuilder(provEx.Message);
                if ((provEx.InnerException != null) && !string.IsNullOrEmpty(provEx.InnerException.Message))
                {
                    builder.AppendFormat(" {0}", provEx.InnerException.Message);
                }
                addError(builder.ToString(), ErrorCode.FailedToRetrieveProviderManifest, EdmSchemaErrorSeverity.Error);
            }

            private void InitializeProviderManifest(Action<string, ErrorCode, EdmSchemaErrorSeverity> addError)
            {
                if (((this._providerManifest == null) && (this._providerManifestToken != null)) && (this._provider != null))
                {
                    DbProviderFactory providerFactory = null;
                    try
                    {
                        providerFactory = DbProviderServices.GetProviderFactory(this._provider);
                    }
                    catch (ArgumentException exception)
                    {
                        addError(exception.Message, ErrorCode.InvalidProvider, EdmSchemaErrorSeverity.Error);
                        return;
                    }
                    try
                    {
                        this._providerManifest = DbProviderServices.GetProviderServices(providerFactory).GetProviderManifest(this._providerManifestToken);
                        this._providerFactory = providerFactory;
                        if (this._providerManifest is EdmProviderManifest)
                        {
                            if (this._throwOnError)
                            {
                                throw EntityUtil.NotSupported(System.Data.Entity.Strings.OnlyStoreConnectionsSupported);
                            }
                            addError(System.Data.Entity.Strings.OnlyStoreConnectionsSupported, ErrorCode.InvalidProvider, EdmSchemaErrorSeverity.Error);
                        }
                    }
                    catch (ProviderIncompatibleException exception2)
                    {
                        if (this._throwOnError)
                        {
                            throw;
                        }
                        this.AddProviderIncompatibleError(exception2, addError);
                    }
                }
            }

            private void LoadItems(IEnumerable<XmlReader> xmlReaders, IEnumerable<string> sourceFilePaths)
            {
                this._errors = SchemaManager.ParseAndValidate(xmlReaders, sourceFilePaths, SchemaDataModelOption.ProviderDataModel, new AttributeValueNotification(this.OnProviderNotification), new AttributeValueNotification(this.OnProviderManifestTokenNotification), new ProviderManifestNeeded(this.OnProviderManifestNeeded), out this._schemas);
                if (this._throwOnError)
                {
                    this.ThrowOnNonWarningErrors();
                }
            }

            private DbProviderManifest OnProviderManifestNeeded(Action<string, ErrorCode, EdmSchemaErrorSeverity> addError)
            {
                if (this._providerManifest == null)
                {
                    addError(System.Data.Entity.Strings.ProviderManifestTokenNotFound, ErrorCode.ProviderManifestTokenNotFound, EdmSchemaErrorSeverity.Error);
                }
                return this._providerManifest;
            }

            private void OnProviderManifestTokenNotification(string token, Action<string, ErrorCode, EdmSchemaErrorSeverity> addError)
            {
                if (this._providerManifestToken == null)
                {
                    this._providerManifestToken = token;
                    this.InitializeProviderManifest(addError);
                }
                else if (this._providerManifestToken != token)
                {
                    addError(System.Data.Entity.Strings.AllArtifactsMustTargetSameProvider_ManifestToken(token, this._providerManifestToken), ErrorCode.ProviderManifestTokenMismatch, EdmSchemaErrorSeverity.Error);
                }
            }

            private void OnProviderNotification(string provider, Action<string, ErrorCode, EdmSchemaErrorSeverity> addError)
            {
                string str = this._provider;
                if (this._provider == null)
                {
                    this._provider = provider;
                    this.InitializeProviderManifest(addError);
                }
                else if (this._provider != provider)
                {
                    addError(System.Data.Entity.Strings.AllArtifactsMustTargetSameProvider_InvariantName(str, this._provider), ErrorCode.InconsistentProvider, EdmSchemaErrorSeverity.Error);
                }
            }

            internal void ThrowOnNonWarningErrors()
            {
                if (!MetadataHelper.CheckIfAllErrorsAreWarnings(this._errors))
                {
                    throw EntityUtil.InvalidSchemaEncountered(Helper.CombineErrorMessage(this._errors));
                }
            }

            public IList<EdmSchemaError> Errors =>
                this._errors;

            public bool HasNonWarningErrors =>
                !MetadataHelper.CheckIfAllErrorsAreWarnings(this._errors);

            public DbProviderFactory ProviderFactory =>
                this._providerFactory;

            public DbProviderManifest ProviderManifest =>
                this._providerManifest;

            public IList<Schema> Schemas =>
                this._schemas;
        }
    }
}

