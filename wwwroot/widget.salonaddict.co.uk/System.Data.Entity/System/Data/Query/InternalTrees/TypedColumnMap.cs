namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;

    internal abstract class TypedColumnMap : StructuredColumnMap
    {
        internal TypedColumnMap(TypeUsage type, string name, ColumnMap[] properties) : base(type, name, properties)
        {
        }
    }
}

