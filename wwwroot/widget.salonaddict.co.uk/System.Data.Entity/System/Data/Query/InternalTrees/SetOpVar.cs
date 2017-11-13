namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;

    internal sealed class SetOpVar : Var
    {
        internal SetOpVar(int id, TypeUsage type) : base(id, VarType.SetOp, type)
        {
        }
    }
}

