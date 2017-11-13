namespace System.Data.Linq.Mapping
{
    using System;
    using System.Reflection;

    public abstract class MetaTable
    {
        protected MetaTable()
        {
        }

        public abstract MethodInfo DeleteMethod { get; }

        public abstract MethodInfo InsertMethod { get; }

        public abstract MetaModel Model { get; }

        public abstract MetaType RowType { get; }

        public abstract string TableName { get; }

        public abstract MethodInfo UpdateMethod { get; }
    }
}

