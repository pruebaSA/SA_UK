namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;

    internal sealed class ComputedVar : Var
    {
        internal ComputedVar(int id, TypeUsage type) : base(id, VarType.Computed, type)
        {
        }
    }
}

