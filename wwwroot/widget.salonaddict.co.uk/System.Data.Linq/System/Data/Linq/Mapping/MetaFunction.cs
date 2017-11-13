namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;

    public abstract class MetaFunction
    {
        protected MetaFunction()
        {
        }

        public abstract bool HasMultipleResults { get; }

        public abstract bool IsComposable { get; }

        public abstract string MappedName { get; }

        public abstract MethodInfo Method { get; }

        public abstract MetaModel Model { get; }

        public abstract string Name { get; }

        public abstract ReadOnlyCollection<MetaParameter> Parameters { get; }

        public abstract ReadOnlyCollection<MetaType> ResultRowTypes { get; }

        public abstract MetaParameter ReturnParameter { get; }
    }
}

