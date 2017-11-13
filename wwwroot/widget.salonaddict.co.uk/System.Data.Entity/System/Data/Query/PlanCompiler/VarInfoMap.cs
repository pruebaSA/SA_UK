namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal class VarInfoMap
    {
        private Dictionary<Var, System.Data.Query.PlanCompiler.VarInfo> m_map = new Dictionary<Var, System.Data.Query.PlanCompiler.VarInfo>();

        internal VarInfoMap()
        {
        }

        internal System.Data.Query.PlanCompiler.VarInfo CreateCollectionVarInfo(Var v, Var newVar)
        {
            System.Data.Query.PlanCompiler.VarInfo info = new CollectionVarInfo(newVar);
            this.m_map.Add(v, info);
            return info;
        }

        internal System.Data.Query.PlanCompiler.VarInfo CreateStructuredVarInfo(Var v, RowType newType, List<Var> newVars, List<EdmProperty> newProperties)
        {
            System.Data.Query.PlanCompiler.VarInfo info = new StructuredVarInfo(newType, newVars, newProperties);
            this.m_map.Add(v, info);
            return info;
        }

        internal bool TryGetVarInfo(Var v, out System.Data.Query.PlanCompiler.VarInfo varInfo) => 
            this.m_map.TryGetValue(v, out varInfo);
    }
}

