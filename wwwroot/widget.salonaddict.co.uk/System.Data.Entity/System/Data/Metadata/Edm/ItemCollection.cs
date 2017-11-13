namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public abstract class ItemCollection : ReadOnlyMetadataCollection<GlobalItem>
    {
        private Dictionary<string, ReadOnlyCollection<EdmFunction>> _functionLookUpTable;
        private readonly System.Data.Metadata.Edm.DataSpace _space;

        internal ItemCollection(System.Data.Metadata.Edm.DataSpace dataspace) : base(new MetadataCollection<GlobalItem>())
        {
            this._space = dataspace;
        }

        internal void AddInternal(GlobalItem item)
        {
            base.Source.Add(item);
        }

        internal bool AtomicAddRange(List<GlobalItem> items) => 
            base.Source.AtomicAddRange(items);

        private static string ComputeFunctionIdentity(string functionName, TypeUsage[] functionParameters) => 
            EdmFunction.BuildIdentity(functionName, ConvertTypeUsageEnumerableToEdmTypeEnumerable(functionParameters));

        private static IEnumerable<EdmType> ConvertTypeUsageEnumerableToEdmTypeEnumerable(IEnumerable<TypeUsage> typeUsages)
        {
            foreach (TypeUsage iteratorVariable0 in typeUsages)
            {
                yield return iteratorVariable0.EdmType;
            }
        }

        internal static ReadOnlyCollection<EdmFunction> GetCaseSensitiveFunctions(ReadOnlyCollection<EdmFunction> functionOverloads, string functionName)
        {
            List<EdmFunction> list = new List<EdmFunction>(functionOverloads.Count);
            for (int i = 0; i < functionOverloads.Count; i++)
            {
                if (functionOverloads[i].FullName == functionName)
                {
                    list.Add(functionOverloads[i]);
                }
            }
            if (list.Count != functionOverloads.Count)
            {
                functionOverloads = list.AsReadOnly();
            }
            return functionOverloads;
        }

        public EntityContainer GetEntityContainer(string name)
        {
            EntityUtil.GenericCheckArgumentNull<string>(name, "name");
            return this.GetEntityContainer(name, false);
        }

        public EntityContainer GetEntityContainer(string name, bool ignoreCase)
        {
            EntityContainer container = this.GetValue(name, ignoreCase) as EntityContainer;
            if (container == null)
            {
                throw EntityUtil.ItemInvalidIdentity(name, "name");
            }
            return container;
        }

        public ReadOnlyCollection<EdmFunction> GetFunctions(string functionName) => 
            this.GetFunctions(functionName, false);

        public ReadOnlyCollection<EdmFunction> GetFunctions(string functionName, bool ignoreCase) => 
            GetFunctions(this.FunctionLookUpTable, functionName, ignoreCase);

        protected static ReadOnlyCollection<EdmFunction> GetFunctions(Dictionary<string, ReadOnlyCollection<EdmFunction>> functionCollection, string functionName, bool ignoreCase)
        {
            ReadOnlyCollection<EdmFunction> onlys;
            if (!functionCollection.TryGetValue(functionName, out onlys))
            {
                return Helper.EmptyEdmFunctionReadOnlyCollection;
            }
            if (ignoreCase)
            {
                return onlys;
            }
            return GetCaseSensitiveFunctions(onlys, functionName);
        }

        public T GetItem<T>(string identity) where T: GlobalItem => 
            this.GetItem<T>(identity, false);

        public T GetItem<T>(string identity, bool ignoreCase) where T: GlobalItem
        {
            T local;
            if (!this.TryGetItem<T>(identity, ignoreCase, out local))
            {
                throw EntityUtil.ItemInvalidIdentity(identity, "identity");
            }
            return local;
        }

        public ReadOnlyCollection<T> GetItems<T>() where T: GlobalItem
        {
            List<T> list = new List<T>();
            foreach (GlobalItem item in this)
            {
                T local = item as T;
                if (local != null)
                {
                    list.Add(local);
                }
            }
            return list.AsReadOnly();
        }

        internal virtual PrimitiveType GetMappedPrimitiveType(PrimitiveTypeKind primitiveTypeKind)
        {
            throw Error.NotSupported();
        }

        public EdmType GetType(string name, string namespaceName) => 
            this.GetType(name, namespaceName, false);

        public EdmType GetType(string name, string namespaceName, bool ignoreCase)
        {
            EntityUtil.GenericCheckArgumentNull<string>(name, "name");
            EntityUtil.GenericCheckArgumentNull<string>(namespaceName, "namespaceName");
            return this.GetItem<EdmType>(EdmType.CreateEdmTypeIdentity(namespaceName, name), ignoreCase);
        }

        internal virtual bool MetadataEquals(ItemCollection other) => 
            object.ReferenceEquals(this, other);

        private static Dictionary<string, ReadOnlyCollection<EdmFunction>> PopulateFunctionLookUpTable(ItemCollection itemCollection)
        {
            Dictionary<string, List<EdmFunction>> dictionary = new Dictionary<string, List<EdmFunction>>(StringComparer.OrdinalIgnoreCase);
            foreach (EdmFunction function in itemCollection.GetItems<EdmFunction>())
            {
                List<EdmFunction> list;
                if (!dictionary.TryGetValue(function.FullName, out list))
                {
                    list = new List<EdmFunction>();
                    dictionary[function.FullName] = list;
                }
                list.Add(function);
            }
            Dictionary<string, ReadOnlyCollection<EdmFunction>> dictionary2 = new Dictionary<string, ReadOnlyCollection<EdmFunction>>(StringComparer.OrdinalIgnoreCase);
            foreach (List<EdmFunction> list2 in dictionary.Values)
            {
                dictionary2.Add(list2[0].FullName, new ReadOnlyCollection<EdmFunction>(list2.ToArray()));
            }
            return dictionary2;
        }

        public bool TryGetEntityContainer(string name, out EntityContainer entityContainer)
        {
            EntityUtil.GenericCheckArgumentNull<string>(name, "name");
            return this.TryGetEntityContainer(name, false, out entityContainer);
        }

        public bool TryGetEntityContainer(string name, bool ignoreCase, out EntityContainer entityContainer)
        {
            EntityUtil.GenericCheckArgumentNull<string>(name, "name");
            GlobalItem item = null;
            if (this.TryGetValue(name, ignoreCase, out item) && Helper.IsEntityContainer(item))
            {
                entityContainer = (EntityContainer) item;
                return true;
            }
            entityContainer = null;
            return false;
        }

        internal bool TryGetFunction(string functionName, TypeUsage[] parameterTypes, bool ignoreCase, out EdmFunction function)
        {
            EntityUtil.GenericCheckArgumentNull<string>(functionName, "functionName");
            EntityUtil.GenericCheckArgumentNull<TypeUsage[]>(parameterTypes, "parameterTypes");
            string identity = ComputeFunctionIdentity(functionName, parameterTypes);
            GlobalItem item = null;
            function = null;
            if (this.TryGetValue(identity, ignoreCase, out item) && Helper.IsEdmFunction(item))
            {
                function = (EdmFunction) item;
                return true;
            }
            return false;
        }

        public bool TryGetItem<T>(string identity, out T item) where T: GlobalItem => 
            this.TryGetItem<T>(identity, false, out item);

        public bool TryGetItem<T>(string identity, bool ignoreCase, out T item) where T: GlobalItem
        {
            GlobalItem item2 = null;
            this.TryGetValue(identity, ignoreCase, out item2);
            item = item2 as T;
            return (((T) item) != null);
        }

        public bool TryGetType(string name, string namespaceName, out EdmType type) => 
            this.TryGetType(name, namespaceName, false, out type);

        public bool TryGetType(string name, string namespaceName, bool ignoreCase, out EdmType type)
        {
            EntityUtil.GenericCheckArgumentNull<string>(name, "name");
            EntityUtil.GenericCheckArgumentNull<string>(namespaceName, "namespaceName");
            GlobalItem item = null;
            this.TryGetValue(EdmType.CreateEdmTypeIdentity(namespaceName, name), ignoreCase, out item);
            type = item as EdmType;
            return (type != null);
        }

        public System.Data.Metadata.Edm.DataSpace DataSpace =>
            this._space;

        internal Dictionary<string, ReadOnlyCollection<EdmFunction>> FunctionLookUpTable
        {
            get
            {
                if (this._functionLookUpTable == null)
                {
                    Dictionary<string, ReadOnlyCollection<EdmFunction>> dictionary = PopulateFunctionLookUpTable(this);
                    Interlocked.CompareExchange<Dictionary<string, ReadOnlyCollection<EdmFunction>>>(ref this._functionLookUpTable, dictionary, null);
                }
                return this._functionLookUpTable;
            }
        }

    }
}

