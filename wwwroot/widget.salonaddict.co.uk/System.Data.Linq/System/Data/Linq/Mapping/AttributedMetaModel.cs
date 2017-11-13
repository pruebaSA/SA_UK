namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq;
    using System.Data.Linq.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    internal class AttributedMetaModel : MetaModel
    {
        private Type contextType;
        private string dbName;
        private bool initFunctions;
        private bool initStaticTables;
        private ReaderWriterLock @lock = new ReaderWriterLock();
        private System.Data.Linq.Mapping.MappingSource mappingSource;
        private Dictionary<MetaPosition, MetaFunction> metaFunctions;
        private Dictionary<Type, MetaTable> metaTables;
        private Dictionary<Type, MetaType> metaTypes;
        private Type providerType;
        private ReadOnlyCollection<MetaTable> staticTables;

        internal AttributedMetaModel(System.Data.Linq.Mapping.MappingSource mappingSource, Type contextType)
        {
            this.mappingSource = mappingSource;
            this.contextType = contextType;
            this.metaTypes = new Dictionary<Type, MetaType>();
            this.metaTables = new Dictionary<Type, MetaTable>();
            this.metaFunctions = new Dictionary<MetaPosition, MetaFunction>();
            ProviderAttribute[] customAttributes = (ProviderAttribute[]) this.contextType.GetCustomAttributes(typeof(ProviderAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length == 1))
            {
                this.providerType = customAttributes[0].Type;
            }
            else
            {
                this.providerType = typeof(SqlProvider);
            }
            DatabaseAttribute[] attributeArray2 = (DatabaseAttribute[]) this.contextType.GetCustomAttributes(typeof(DatabaseAttribute), false);
            this.dbName = ((attributeArray2 != null) && (attributeArray2.Length > 0)) ? attributeArray2[0].Name : this.contextType.Name;
        }

        public override MetaFunction GetFunction(MethodInfo method)
        {
            if (method == null)
            {
                throw System.Data.Linq.Mapping.Error.ArgumentNull("method");
            }
            this.InitFunctions();
            MetaFunction function = null;
            this.metaFunctions.TryGetValue(new MetaPosition(method), out function);
            return function;
        }

        public override IEnumerable<MetaFunction> GetFunctions()
        {
            this.InitFunctions();
            return this.metaFunctions.Values.ToList<MetaFunction>().AsReadOnly();
        }

        public override MetaType GetMetaType(Type type)
        {
            if (type == null)
            {
                throw System.Data.Linq.Mapping.Error.ArgumentNull("type");
            }
            MetaType type2 = null;
            this.@lock.AcquireReaderLock(-1);
            try
            {
                if (this.metaTypes.TryGetValue(type, out type2))
                {
                    return type2;
                }
            }
            finally
            {
                this.@lock.ReleaseReaderLock();
            }
            MetaTable table = this.GetTable(type);
            if (table != null)
            {
                return table.RowType.GetInheritanceType(type);
            }
            this.InitFunctions();
            this.@lock.AcquireWriterLock(-1);
            try
            {
                if (!this.metaTypes.TryGetValue(type, out type2))
                {
                    type2 = new UnmappedType(this, type);
                    this.metaTypes.Add(type, type2);
                }
            }
            finally
            {
                this.@lock.ReleaseWriterLock();
            }
            return type2;
        }

        private static Type GetRoot(Type derivedType)
        {
            while ((derivedType != null) && (derivedType != typeof(object)))
            {
                TableAttribute[] customAttributes = (TableAttribute[]) derivedType.GetCustomAttributes(typeof(TableAttribute), false);
                if (customAttributes.Length > 0)
                {
                    return derivedType;
                }
                derivedType = derivedType.BaseType;
            }
            return null;
        }

        public override MetaTable GetTable(Type rowType)
        {
            MetaTable tableNoLocks;
            if (rowType == null)
            {
                throw System.Data.Linq.Mapping.Error.ArgumentNull("rowType");
            }
            this.@lock.AcquireReaderLock(-1);
            try
            {
                if (this.metaTables.TryGetValue(rowType, out tableNoLocks))
                {
                    return tableNoLocks;
                }
            }
            finally
            {
                this.@lock.ReleaseReaderLock();
            }
            this.@lock.AcquireWriterLock(-1);
            try
            {
                tableNoLocks = this.GetTableNoLocks(rowType);
            }
            finally
            {
                this.@lock.ReleaseWriterLock();
            }
            return tableNoLocks;
        }

        internal MetaTable GetTableNoLocks(Type rowType)
        {
            MetaTable table;
            if (!this.metaTables.TryGetValue(rowType, out table))
            {
                Type key = GetRoot(rowType) ?? rowType;
                TableAttribute[] customAttributes = (TableAttribute[]) key.GetCustomAttributes(typeof(TableAttribute), true);
                if (customAttributes.Length == 0)
                {
                    this.metaTables.Add(rowType, null);
                    return table;
                }
                if (!this.metaTables.TryGetValue(key, out table))
                {
                    table = new AttributedMetaTable(this, customAttributes[0], key);
                    foreach (MetaType type2 in table.RowType.InheritanceTypes)
                    {
                        this.metaTables.Add(type2.Type, table);
                    }
                }
                if (table.RowType.GetInheritanceType(rowType) == null)
                {
                    this.metaTables.Add(rowType, null);
                    return null;
                }
            }
            return table;
        }

        public override IEnumerable<MetaTable> GetTables()
        {
            IEnumerable<MetaTable> enumerable;
            this.InitStaticTables();
            if (this.staticTables.Count > 0)
            {
                return this.staticTables;
            }
            this.@lock.AcquireReaderLock(-1);
            try
            {
                enumerable = (from x in this.metaTables.Values
                    where x != null
                    select x).Distinct<MetaTable>();
            }
            finally
            {
                this.@lock.ReleaseReaderLock();
            }
            return enumerable;
        }

        private void InitFunctions()
        {
            if (!this.initFunctions)
            {
                this.@lock.AcquireWriterLock(-1);
                try
                {
                    if (!this.initFunctions)
                    {
                        if (this.contextType != typeof(DataContext))
                        {
                            for (Type type = this.contextType; type != typeof(DataContext); type = type.BaseType)
                            {
                                foreach (MethodInfo info in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                                {
                                    if (IsUserFunction(info))
                                    {
                                        if (info.IsGenericMethodDefinition)
                                        {
                                            throw System.Data.Linq.Mapping.Error.InvalidUseOfGenericMethodAsMappedFunction(info.Name);
                                        }
                                        MetaPosition key = new MetaPosition(info);
                                        if (!this.metaFunctions.ContainsKey(key))
                                        {
                                            MetaFunction function = new AttributedMetaFunction(this, info);
                                            this.metaFunctions.Add(key, function);
                                            foreach (MetaType type2 in function.ResultRowTypes)
                                            {
                                                foreach (MetaType type3 in type2.InheritanceTypes)
                                                {
                                                    if (!this.metaTypes.ContainsKey(type3.Type))
                                                    {
                                                        this.metaTypes.Add(type3.Type, type3);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        this.initFunctions = true;
                    }
                }
                finally
                {
                    this.@lock.ReleaseWriterLock();
                }
            }
        }

        private void InitStaticTables()
        {
            if (!this.initStaticTables)
            {
                this.@lock.AcquireWriterLock(-1);
                try
                {
                    if (!this.initStaticTables)
                    {
                        HashSet<MetaTable> collection = new HashSet<MetaTable>();
                        for (Type type = this.contextType; type != typeof(DataContext); type = type.BaseType)
                        {
                            foreach (FieldInfo info in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                            {
                                Type fieldType = info.FieldType;
                                if (fieldType.IsGenericType && (fieldType.GetGenericTypeDefinition() == typeof(Table<>)))
                                {
                                    Type rowType = fieldType.GetGenericArguments()[0];
                                    collection.Add(this.GetTableNoLocks(rowType));
                                }
                            }
                            foreach (PropertyInfo info2 in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                            {
                                Type propertyType = info2.PropertyType;
                                if (propertyType.IsGenericType && (propertyType.GetGenericTypeDefinition() == typeof(Table<>)))
                                {
                                    Type type5 = propertyType.GetGenericArguments()[0];
                                    collection.Add(this.GetTableNoLocks(type5));
                                }
                            }
                        }
                        this.staticTables = new List<MetaTable>(collection).AsReadOnly();
                        this.initStaticTables = true;
                    }
                }
                finally
                {
                    this.@lock.ReleaseWriterLock();
                }
            }
        }

        private static bool IsUserFunction(MethodInfo mi) => 
            (Attribute.GetCustomAttribute(mi, typeof(FunctionAttribute), false) != null);

        public override Type ContextType =>
            this.contextType;

        public override string DatabaseName =>
            this.dbName;

        public override System.Data.Linq.Mapping.MappingSource MappingSource =>
            this.mappingSource;

        public override Type ProviderType =>
            this.providerType;
    }
}

