namespace System.Data.Linq.Mapping
{
    using System;
    using System.Reflection;

    public abstract class MetaParameter
    {
        protected MetaParameter()
        {
        }

        public abstract string DbType { get; }

        public abstract string MappedName { get; }

        public abstract string Name { get; }

        public abstract ParameterInfo Parameter { get; }

        public abstract Type ParameterType { get; }
    }
}

