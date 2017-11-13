namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;

    internal abstract class SimpleColumnMap : ColumnMap
    {
        internal SimpleColumnMap(TypeUsage type, string name) : base(type, name)
        {
        }
    }
}

