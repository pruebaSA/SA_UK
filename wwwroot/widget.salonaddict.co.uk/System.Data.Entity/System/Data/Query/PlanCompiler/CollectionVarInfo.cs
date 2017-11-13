namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;

    internal class CollectionVarInfo : System.Data.Query.PlanCompiler.VarInfo
    {
        private List<Var> m_newVars = new List<Var>();

        internal CollectionVarInfo(Var newVar)
        {
            this.m_newVars.Add(newVar);
        }

        internal override bool IsCollectionType =>
            true;

        internal Var NewVar =>
            this.m_newVars[0];

        internal override List<Var> NewVars =>
            this.m_newVars;
    }
}

