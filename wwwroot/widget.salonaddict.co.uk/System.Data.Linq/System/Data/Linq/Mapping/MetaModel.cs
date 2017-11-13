namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class MetaModel
    {
        private object identity = new object();

        protected MetaModel()
        {
        }

        public abstract MetaFunction GetFunction(MethodInfo method);
        public abstract IEnumerable<MetaFunction> GetFunctions();
        public abstract MetaType GetMetaType(Type type);
        public abstract MetaTable GetTable(Type rowType);
        public abstract IEnumerable<MetaTable> GetTables();

        public abstract Type ContextType { get; }

        public abstract string DatabaseName { get; }

        internal object Identity =>
            this.identity;

        public abstract System.Data.Linq.Mapping.MappingSource MappingSource { get; }

        public abstract Type ProviderType { get; }
    }
}

